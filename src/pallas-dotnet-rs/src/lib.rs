use std::collections::HashMap;

use lazy_static::lazy_static;
use pallas::{
    codec::utils::KeepRaw,
    ledger::{
        addresses::{Address, ByronAddress},
        primitives::conway::{PlutusData, PseudoDatumOption},
        traverse::{Era, MultiEraBlock, MultiEraTx},
    },
    network::{
        facades::{Error, NodeClient},
        miniprotocols::{
            chainsync, localstate::queries_v16, localtxsubmission::{EraTx, GenericClient, RejectReason}, Point as PallasPoint, MAINNET_MAGIC, PREVIEW_MAGIC, PRE_PRODUCTION_MAGIC, PROTOCOL_N2C_TX_SUBMISSION, PROTOCOL_N2N_TX_SUBMISSION, TESTNET_MAGIC
        },
        multiplexer::{self, Bearer},
    },
};
use rnet::{net, Net};
use tokio::runtime::Runtime;

rnet::root!();

lazy_static! {
    static ref RT: Runtime = Runtime::new().expect("Failed to create Tokio runtime");
}

const DATUM_TYPE_HASH: u8 = 1;
const DATUM_TYPE_DATA: u8 = 2;

#[derive(Net)]
pub struct NetworkMagic {}

impl NetworkMagic {
    #[net]
    pub fn mainnet_magic() -> u64 {
        MAINNET_MAGIC
    }

    #[net]
    pub fn testnet_magic() -> u64 {
        TESTNET_MAGIC
    }

    #[net]
    pub fn preview_magic() -> u64 {
        PREVIEW_MAGIC
    }

    #[net]
    pub fn pre_production_magic() -> u64 {
        PRE_PRODUCTION_MAGIC
    }
}

#[derive(Net)]
pub struct Point {
    slot: u64,
    hash: Vec<u8>,
}

#[derive(Net)]
pub struct Block {
    slot: u64,
    hash: Vec<u8>,
    number: u64,
    transaction_bodies: Vec<TransactionBody>,
}

#[derive(Net)]
pub struct TransactionBody {
    id: Vec<u8>,
    inputs: Vec<TransactionInput>,
    outputs: Vec<TransactionOutput>,
    index: usize,
}

#[derive(Net)]
pub struct TransactionInput {
    id: Vec<u8>,
    index: u64,
}

#[derive(Net)]
struct Datum {
    datum_type: u8,
    data: Option<Vec<u8>>,
}

#[derive(Net)]
pub struct TransactionOutput {
    address: Vec<u8>,
    amount: Value,
    index: usize,
    datum: Option<Datum>,
}

#[derive(Net)]
pub struct Value {
    coin: Coin,
    multi_asset: HashMap<PolicyId, HashMap<AssetName, Coin>>,
}

pub type Coin = u64;
pub type PolicyId = Vec<u8>;
pub type AssetName = Vec<u8>;

#[derive(Net)]
pub struct NextResponse {
    action: u8,
    tip: Option<Block>,
    block: Option<Block>,
}

pub struct NodeClientWrapperData {
    client_ptr: usize,
    socket_path: String,
}

#[derive(Net)]
pub struct NodeClientWrapper {
    client_data_ptr: usize,
}

impl NodeClientWrapper {
    #[net]
    pub fn connect(socket_path: String, network_magic: u64) -> NodeClientWrapper {
        NodeClientWrapper::connect(socket_path, network_magic)
    }

    pub fn connect(socket_path: String, network_magic: u64) -> NodeClientWrapper {
        let client = RT.block_on(async {
            NodeClient::connect(&socket_path, network_magic)
                .await
                .unwrap()
        });

        let client_box = Box::new(client);
        let client_ptr = Box::into_raw(client_box) as usize;
        let client_wrapper_data = NodeClientWrapperData {
            client_ptr,
            socket_path,
        };

        let client_data_ptr = Box::into_raw(Box::new(client_wrapper_data)) as usize;

        NodeClientWrapper { client_data_ptr }
    }

    #[net]
    pub fn get_tip(client_wrapper: NodeClientWrapper) -> Point {
        unsafe {
            let client_data_ptr = client_wrapper.client_data_ptr as *mut NodeClientWrapperData;
            let client_data = Box::from_raw(client_data_ptr);

            let client_ptr = client_data.client_ptr as *mut NodeClient;

            // Convert the raw pointer back to a Box to deallocate the memory
            let mut client = Box::from_raw(client_ptr);

            // Get the tip
            let tip = RT.block_on(async {
                let state_query_client = client.statequery();

                state_query_client.acquire(None).await.unwrap();

                queries_v16::get_chain_point(state_query_client)
                    .await
                    .unwrap()
            });

            // Convert client back to a raw pointer for future use
            let _ = Box::into_raw(client);
            let _ = Box::into_raw(client_data);

            match tip {
                PallasPoint::Origin => Point {
                    slot: 0,
                    hash: vec![],
                },
                PallasPoint::Specific(slot, hash) => Point { slot, hash },
            }
        }
    }

    #[net]
    pub fn find_intersect(client_wrapper: NodeClientWrapper, known_point: Point) -> Option<Point> {
        NodeClientWrapper::find_intersect(client_wrapper, known_point)
    }

    pub fn find_intersect(
        client_wrapper: NodeClientWrapper,
        known_point: Point,
    ) -> Option<Point> {
        unsafe {
            let client_data_ptr = client_wrapper.client_data_ptr as *mut NodeClientWrapperData;
            let client_data = Box::from_raw(client_data_ptr);

            let client_ptr = client_data.client_ptr as *mut NodeClient;

            // Convert the raw pointer back to a Box to deallocate the memory
            let mut _client = Box::from_raw(client_ptr);
            let client = _client.chainsync();

            let known_points = vec![PallasPoint::Specific(known_point.slot, known_point.hash)];

            // Get the intersecting point and the tip
            let (intersect_point, _tip) =
                RT.block_on(async { client.find_intersect(known_points).await.unwrap() });

            // Convert client back to a raw pointer for future use
            let _ = Box::into_raw(_client);
            let _ = Box::into_raw(client_data);

            // Match on the intersecting point
            intersect_point.map(|pallas_point| match pallas_point {
                PallasPoint::Origin => Point {
                    slot: 0,
                    hash: vec![],
                },
                PallasPoint::Specific(slot, hash) => Point { slot, hash },
            })
        }
    }

    #[net]
    pub fn chain_sync_next(client_wrapper: NodeClientWrapper) -> NextResponse {
        unsafe {
            let client_data_ptr = client_wrapper.client_data_ptr as *mut NodeClientWrapperData;
            let client_data = Box::from_raw(client_data_ptr);

            let client_ptr = client_data.client_ptr as *mut NodeClient;

            // Convert the raw pointer back to a Box to deallocate the memory
            let mut client = Box::from_raw(client_ptr);

            // Get the next block
            let result = RT.block_on(async {
                client.chainsync().request_next().await
            });

            let next_response = match result {
                Ok(next) => match next {
                    chainsync::NextResponse::RollForward(h, tip) => match MultiEraBlock::decode(&h)
                    {
                        Ok(b) => NextResponse {
                            action: 1,
                            tip: match tip.0 {
                                PallasPoint::Origin => Some(Block {
                                    slot: 0,
                                    hash: vec![],
                                    number: 0,
                                    transaction_bodies: vec![],
                                }),
                                PallasPoint::Specific(slot, hash) => Some(Block {
                                    slot,
                                    hash,
                                    number: tip.1,
                                    transaction_bodies: vec![],
                                }),
                            },
                            block: Some(Block {
                                slot: b.slot(),
                                hash: b.hash().to_vec(),
                                number: b.number(),
                                transaction_bodies: b
                                    .txs()
                                    .into_iter()
                                    .enumerate()
                                    .map(|(index, tx_body)| TransactionBody {
                                        id: tx_body.hash().to_vec(),
                                        index,
                                        inputs: tx_body
                                            .inputs()
                                            .into_iter()
                                            .map(|tx_input| TransactionInput {
                                                id: tx_input.hash().to_vec(),
                                                index: tx_input.index(),
                                            })
                                            .collect(),
                                        outputs: tx_body
                                            .outputs()
                                            .into_iter()
                                            .enumerate()
                                            .map(|(index, tx_output)| TransactionOutput {
                                                index,
                                                address: tx_output.address().unwrap().to_vec(),
                                                datum: tx_output.datum().map(convert_to_datum),
                                                amount: Value {
                                                    coin: tx_output.lovelace_amount(),
                                                    multi_asset: tx_output
                                                        .non_ada_assets()
                                                        .iter()
                                                        .filter(|ma| ma.is_output())
                                                        .map(|ma| {
                                                            (
                                                                ma.policy().to_vec(),
                                                                ma.assets()
                                                                    .iter()
                                                                    .map(|a| {
                                                                        (
                                                                            a.name().to_vec(),
                                                                            a.output_coin()
                                                                                .unwrap(),
                                                                        )
                                                                    })
                                                                    .collect(),
                                                            )
                                                        })
                                                        .collect(),
                                                },
                                            })
                                            .collect(),
                                    })
                                    .collect(),
                            }),
                        },
                        Err(e) => {
                            println!("error: {:?}", e);
                            NextResponse {
                                action: 0,
                                block: None,
                                tip: None,
                            }
                        }
                    },
                    chainsync::NextResponse::RollBackward(point, tip) => NextResponse {
                        action: 2,
                        tip: match tip.0 {
                            PallasPoint::Origin => Some(Block {
                                slot: 0,
                                hash: vec![],
                                number: 0,
                                transaction_bodies: vec![],
                            }),
                            PallasPoint::Specific(slot, hash) => Some(Block {
                                slot,
                                hash,
                                number: tip.1,
                                transaction_bodies: vec![],
                            }),
                        },
                        block: match point {
                            PallasPoint::Origin => Some(Block {
                                slot: 0,
                                hash: vec![],
                                number: 0,
                                transaction_bodies: vec![],
                            }),
                            PallasPoint::Specific(slot, hash) => Some(Block {
                                slot,
                                hash,
                                number: 0,
                                transaction_bodies: vec![],
                            }),
                        },
                    },
                    chainsync::NextResponse::Await => NextResponse {
                        action: 3,
                        tip: None,
                        block: None,
                    },
                },
                Err(e) => {
                    println!("chain_sync_next error: {:?}", e);
                    NextResponse {
                        action: 0,
                        block: None,
                        tip: None,
                    }
                }
            };

            // Convert client back to a raw pointer for future use

            let _ = Box::into_raw(client);
            let _ = Box::into_raw(client_data);
            next_response
        }
    }

    #[net]
    pub fn chain_sync_has_agency(client_wrapper: NodeClientWrapper) -> bool {
        unsafe {
            let client_data_ptr = client_wrapper.client_data_ptr as *mut NodeClientWrapperData;
            let client_data = Box::from_raw(client_data_ptr);

            let client_ptr = client_data.client_ptr as *mut NodeClient;

            // Convert the raw pointer back to a Box to deallocate the memory
            let mut _client = Box::from_raw(client_ptr);

            let has_agency = _client.chainsync().has_agency();

            // Convert client back to a raw pointer for future use
            let _ = Box::into_raw(_client);
            let _ = Box::into_raw(client_data);

            has_agency
        }
    }

    #[net]
    pub fn submit_tx(client_wrapper: NodeClientWrapper, tx: Vec<u8>) -> bool {
        NodeClientWrapper::submit_tx(client_wrapper, tx)
    }

    pub fn submit_tx(client_wrapper: NodeClientWrapper, tx: Vec<u8>) -> bool {
        unsafe {
            let client_data_ptr = client_wrapper.client_data_ptr as *mut NodeClientWrapperData;
            let client_data = Box::from_raw(client_data_ptr);

            let client_ptr = client_data.client_ptr as *mut NodeClient;

            // Convert the raw pointer back to a Box to deallocate the memory
            let mut _client = Box::from_raw(client_ptr);
            RT.block_on(async {
                let bearer = Bearer::connect_unix(&client_data.socket_path)
                    .await
                    .map_err(Error::ConnectFailure)
                    .unwrap();

                let mut plexer = multiplexer::Plexer::new(bearer);
                let txsubmit_channel = plexer.subscribe_client(PROTOCOL_N2N_TX_SUBMISSION);
                let mut txsubmit_client: GenericClient<EraTx, RejectReason> =
                    GenericClient::new(txsubmit_channel);
                let decoded_tx = MultiEraTx::decode(&tx).unwrap();
                let era_tx = EraTx(to_u16(decoded_tx.era()), tx);
                txsubmit_client.submit_tx(era_tx).await.unwrap();
            });

            // Convert client back to a raw pointer for future use
            let _ = Box::into_raw(_client);
            let _ = Box::into_raw(client_data);

            true
        }
    }

    #[net]
    pub fn disconnect(client_wrapper: NodeClientWrapper) {
        unsafe {
            let client_data_ptr = client_wrapper.client_data_ptr as *mut NodeClientWrapperData;
            let client_data = Box::from_raw(client_data_ptr);

            let client_ptr = client_data.client_ptr as *mut NodeClient;

            let mut _client = Box::from_raw(client_ptr);

            _client.abort();
        }
    }
}

pub fn to_u16(era: Era) -> u16 {
    match era {
        Era::Byron => 0,
        Era::Shelley => 1,
        Era::Allegra => 2,
        Era::Mary => 3,
        Era::Alonzo => 4,
        Era::Babbage => 5,
        Era::Conway => 6,
        _ => 7, // Assume a future era
    }
}

fn convert_to_datum(datum: PseudoDatumOption<KeepRaw<'_, PlutusData>>) -> Datum {
    match datum {
        PseudoDatumOption::Hash(hash) => Datum {
            datum_type: DATUM_TYPE_HASH,
            data: Some(hash.to_vec()),
        },
        PseudoDatumOption::Data(keep_raw) => {
            let raw_data = keep_raw.raw_cbor().to_vec();
            Datum {
                datum_type: DATUM_TYPE_DATA,
                data: Some(raw_data),
            }
        }
    }
}

#[derive(Net)]
pub struct PallasUtility {}

impl PallasUtility {
    #[net]
    pub fn address_bytes_to_bech32(address_bytes: Vec<u8>) -> String {
        match Address::from_bytes(&address_bytes).unwrap().to_bech32() {
            Ok(address) => address,
            Err(_) => ByronAddress::from_bytes(&address_bytes)
                .unwrap()
                .to_base58(),
        }
    }
}

mod tests {
    use super::*;

    #[test]
    fn test_find_intersect() {
        let client_wrapper =
            NodeClientWrapper::connect("/tmp/node.socket".to_string(), PREVIEW_MAGIC);
        let block_hash =
            hex::decode("a9e99c93352f91233a61fb55da83a43c49abf1c84a636e226e11be5ac0343dc3")
                .unwrap();
        let intersect = NodeClientWrapper::find_intersect(
            client_wrapper,
            Point {
                slot: 35197575,
                hash: block_hash.clone(),
            },
        )
        .unwrap();
        assert_eq!(intersect.slot, 35197575);
        assert_eq!(intersect.hash, block_hash);
    }

    #[test]
    fn test_tx_submit() {
        let client_wrapper =
            NodeClientWrapper::connect("/tmp/node.socket".to_string(), PREVIEW_MAGIC);
        let tx = hex::decode("84A300828258207833C101F6A0F80F8BFDD6D90184CAB2A551CE9A595C0D4635E403FFDC31EA55018258207833C101F6A0F80F8BFDD6D90184CAB2A551CE9A595C0D4635E403FFDC31EA55020183A300581D7035FEB70954E97F2601B84C734D2A5315EE0A5E321E4B4F0E489C4CB201821A004C4B40A1581C8B05E87A51C1D4A0FA888D2BB14DBC25E8C343EA379A171B63AA84A0A144434E43541A000186A0028201D81858CBD8799FD8799F581CE63022B0F461602484968BB10FD8F872787B862ACE2D7E943292A370FFD8799FD8799FD8799F581CE63022B0F461602484968BB10FD8F872787B862ACE2D7E943292A370FFD8799FD8799FD8799F581C03EC6A12860EF8C07D4C1A8DE7DF06ACB0F0330A6087ECBE972082A7FFFFFFFFD8799FFFFF1A000493E0D8799F051864FF581C8B05E87A51C1D4A0FA888D2BB14DBC25E8C343EA379A171B63AA84A044434E4354581C5496B3318F8CA933BBFDF19B8FAA7F948D044208E0278D62C24EE73EFF82583900E63022B0F461602484968BB10FD8F872787B862ACE2D7E943292A37003EC6A12860EF8C07D4C1A8DE7DF06ACB0F0330A6087ECBE972082A7821A0011A008A1581C8B05E87A51C1D4A0FA888D2BB14DBC25E8C343EA379A171B63AA84A0A144434E43541A00030D4082583900E63022B0F461602484968BB10FD8F872787B862ACE2D7E943292A37003EC6A12860EF8C07D4C1A8DE7DF06ACB0F0330A6087ECBE972082A71B00000004A345246C021A0002D039A100818258206395DDF277DCCA535AAB1C5C65680E8C73AAE3AB883060EF88C45A2DDE1287325840653EB36C0C6019599F7E8376BC458C5C8774BEC66BE951D09BC4009753C58A297E6D8EA5318CBFEC6F1C11E2A88F234B2F467E64C7D089D350E5CFCCA9E56401F5F6");
        let result = NodeClientWrapper::submit_tx(client_wrapper, tx.unwrap());
        assert!(result);
    }
}
