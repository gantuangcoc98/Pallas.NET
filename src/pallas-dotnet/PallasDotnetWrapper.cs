using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PallasDotnetRs
{    
    public static class PallasDotnetRs
    {
        public class RustException: Exception {
            public RustException(string message) : base(message) { }
        }

        public interface IOpaqueHandle: IEquatable<IOpaqueHandle>, IDisposable {}

        
        public struct NetworkMagic {
        }
        public struct Point {
            public ulong slot;
            public List<byte> hash;
        }
        public struct Block {
            public ulong slot;
            public List<byte> hash;
            public ulong number;
            public List<TransactionBody> transactionBodies;
        }
        public struct TransactionBody {
            public List<byte> id;
            public ushort era;
            public List<TransactionInput> inputs;
            public List<TransactionOutput> outputs;
            public Dictionary<List<byte>,Dictionary<List<byte>,long>> mint;
            public UIntPtr index;
            public string metadata;
            public List<Redeemer> redeemers;
            public List<byte> raw;
        }
        public struct TransactionInput {
            public List<byte> id;
            public ulong index;
        }
        public struct Datum {
            public byte datumType;
            public List<byte> data;
        }
        public struct TransactionOutput {
            public List<byte> address;
            public Value amount;
            public UIntPtr index;
            public Datum datum;
            public List<byte> raw;
        }
        public struct Value {
            public ulong coin;
            public Dictionary<List<byte>,Dictionary<List<byte>,ulong>> multiAsset;
        }
        public struct Redeemer {
            public byte tag;
            public uint index;
            public List<byte> data;
            public ExUnits exUnits;
        }
        public struct ExUnits {
            public ulong mem;
            public ulong steps;
        }
        public struct NextResponse {
            public byte action;
            public Block tip;
            public Block block;
        }
        public struct NodeClientWrapper {
            public UIntPtr clientDataPtr;
        }
        public struct PallasUtility {
        }
        public struct TxSubmit {
        }
        public static ulong MainnetMagic(
        ) {
            return _FnMainnetMagic();
        }
        public static ulong TestnetMagic(
        ) {
            return _FnTestnetMagic();
        }
        public static ulong PreviewMagic(
        ) {
            return _FnPreviewMagic();
        }
        public static ulong PreProductionMagic(
        ) {
            return _FnPreProductionMagic();
        }
        public static NodeClientWrapper Connect(
            string socketPath,
            ulong networkMagic
        ) {
            return (_FnConnect(_AllocStr(socketPath),networkMagic)).Decode();
        }
        public static List<List<byte>> GetUtxoByAddressCbor(
            NodeClientWrapper clientWrapper,
            string address
        ) {
            return _FreeSlice<List<byte>, _RawSlice, List<List<byte>>>(_FnGetUtxoByAddressCbor(_StructNodeClientWrapper.Encode(clientWrapper),_AllocStr(address)), 16, 8, _arg1 => _FreeSlice<byte, byte, List<byte>>(_arg1, 1, 1, _arg2 => _arg2));
        }
        public static Point GetTip(
            NodeClientWrapper clientWrapper
        ) {
            return (_FnGetTip(_StructNodeClientWrapper.Encode(clientWrapper))).Decode();
        }
        public static Point FindIntersect(
            NodeClientWrapper clientWrapper,
            Point knownPoint
        ) {
            return _DecodeOption(_FnFindIntersect(_StructNodeClientWrapper.Encode(clientWrapper),_StructPoint.Encode(knownPoint)), _arg3 => (_arg3).Decode());
        }
        public static NextResponse ChainSyncNext(
            NodeClientWrapper clientWrapper
        ) {
            return (_FnChainSyncNext(_StructNodeClientWrapper.Encode(clientWrapper))).Decode();
        }
        public static bool ChainSyncHasAgency(
            NodeClientWrapper clientWrapper
        ) {
            return (_FnChainSyncHasAgency(_StructNodeClientWrapper.Encode(clientWrapper)) != 0);
        }
        public static void Disconnect(
            NodeClientWrapper clientWrapper
        ) {
            _FnDisconnect(_StructNodeClientWrapper.Encode(clientWrapper));
        }
        public static string AddressBytesToBech32(
            IReadOnlyCollection<byte> addressBytes
        ) {
            return _FreeStr(_FnAddressBytesToBech32(_AllocSlice<byte, byte>(addressBytes, 1, 1, _arg4 => _arg4)));
        }
        public static List<byte> SubmitTx(
            string server,
            ulong magic,
            IReadOnlyCollection<byte> tx
        ) {
            return _FreeSlice<byte, byte, List<byte>>(_FnSubmitTx(_AllocStr(server),magic,_AllocSlice<byte, byte>(tx, 1, 1, _arg5 => _arg5)), 1, 1, _arg6 => _arg6);
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct _StructNetworkMagic {
            public static _StructNetworkMagic Encode(NetworkMagic structArg) {
                return new _StructNetworkMagic {
                };
            }
            public NetworkMagic Decode() {
                return new NetworkMagic {
                };
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct _StructPoint {
            public ulong slot;
            public _RawSlice hash;
            public static _StructPoint Encode(Point structArg) {
                return new _StructPoint {
                    slot = structArg.slot,
                    hash = _AllocSlice<byte, byte>(structArg.hash, 1, 1, _arg7 => _arg7)
                };
            }
            public Point Decode() {
                return new Point {
                    slot = this.slot,
                    hash = _FreeSlice<byte, byte, List<byte>>(this.hash, 1, 1, _arg8 => _arg8)
                };
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct _StructBlock {
            public ulong slot;
            public _RawSlice hash;
            public ulong number;
            public _RawSlice transactionBodies;
            public static _StructBlock Encode(Block structArg) {
                return new _StructBlock {
                    slot = structArg.slot,
                    hash = _AllocSlice<byte, byte>(structArg.hash, 1, 1, _arg9 => _arg9),
                    number = structArg.number,
                    transactionBodies = _AllocSlice<TransactionBody, _StructTransactionBody>(structArg.transactionBodies, 128, 8, _arg10 => _StructTransactionBody.Encode(_arg10))
                };
            }
            public Block Decode() {
                return new Block {
                    slot = this.slot,
                    hash = _FreeSlice<byte, byte, List<byte>>(this.hash, 1, 1, _arg11 => _arg11),
                    number = this.number,
                    transactionBodies = _FreeSlice<TransactionBody, _StructTransactionBody, List<TransactionBody>>(this.transactionBodies, 128, 8, _arg12 => (_arg12).Decode())
                };
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct _StructTransactionBody {
            public _RawSlice id;
            public ushort era;
            public _RawSlice inputs;
            public _RawSlice outputs;
            public _RawSlice mint;
            public UIntPtr index;
            public _RawSlice metadata;
            public _RawSlice redeemers;
            public _RawSlice raw;
            public static _StructTransactionBody Encode(TransactionBody structArg) {
                return new _StructTransactionBody {
                    id = _AllocSlice<byte, byte>(structArg.id, 1, 1, _arg13 => _arg13),
                    era = structArg.era,
                    inputs = _AllocSlice<TransactionInput, _StructTransactionInput>(structArg.inputs, 24, 8, _arg14 => _StructTransactionInput.Encode(_arg14)),
                    outputs = _AllocSlice<TransactionOutput, _StructTransactionOutput>(structArg.outputs, 104, 8, _arg15 => _StructTransactionOutput.Encode(_arg15)),
                    mint = _AllocDict<List<byte>, Dictionary<List<byte>,long>, _RawTuple0>(structArg.mint, 32, 8, _arg16 => ((Func<(List<byte>,Dictionary<List<byte>,long>), _RawTuple0>)(_arg17 => new _RawTuple0 { elem0 = _AllocSlice<byte, byte>(_arg17.Item1, 1, 1, _arg18 => _arg18),elem1 = _AllocDict<List<byte>, long, _RawTuple1>(_arg17.Item2, 24, 8, _arg19 => ((Func<(List<byte>,long), _RawTuple1>)(_arg20 => new _RawTuple1 { elem0 = _AllocSlice<byte, byte>(_arg20.Item1, 1, 1, _arg21 => _arg21),elem1 = _arg20.Item2 }))(_arg19)) }))(_arg16)),
                    index = structArg.index,
                    metadata = _AllocStr(structArg.metadata),
                    redeemers = _AllocSlice<Redeemer, _StructRedeemer>(structArg.redeemers, 40, 8, _arg22 => _StructRedeemer.Encode(_arg22)),
                    raw = _AllocSlice<byte, byte>(structArg.raw, 1, 1, _arg23 => _arg23)
                };
            }
            public TransactionBody Decode() {
                return new TransactionBody {
                    id = _FreeSlice<byte, byte, List<byte>>(this.id, 1, 1, _arg24 => _arg24),
                    era = this.era,
                    inputs = _FreeSlice<TransactionInput, _StructTransactionInput, List<TransactionInput>>(this.inputs, 24, 8, _arg25 => (_arg25).Decode()),
                    outputs = _FreeSlice<TransactionOutput, _StructTransactionOutput, List<TransactionOutput>>(this.outputs, 104, 8, _arg26 => (_arg26).Decode()),
                    mint = _FreeDict<List<byte>, Dictionary<List<byte>,long>, _RawTuple0, Dictionary<List<byte>, Dictionary<List<byte>,long>>>(this.mint, 32, 8, _arg27 => ((Func<_RawTuple0, (List<byte>,Dictionary<List<byte>,long>)>)(_arg28 => (_FreeSlice<byte, byte, List<byte>>(_arg28.elem0, 1, 1, _arg29 => _arg29),_FreeDict<List<byte>, long, _RawTuple1, Dictionary<List<byte>, long>>(_arg28.elem1, 24, 8, _arg30 => ((Func<_RawTuple1, (List<byte>,long)>)(_arg31 => (_FreeSlice<byte, byte, List<byte>>(_arg31.elem0, 1, 1, _arg32 => _arg32),_arg31.elem1)))(_arg30)))))(_arg27)),
                    index = this.index,
                    metadata = _FreeStr(this.metadata),
                    redeemers = _FreeSlice<Redeemer, _StructRedeemer, List<Redeemer>>(this.redeemers, 40, 8, _arg33 => (_arg33).Decode()),
                    raw = _FreeSlice<byte, byte, List<byte>>(this.raw, 1, 1, _arg34 => _arg34)
                };
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct _StructTransactionInput {
            public _RawSlice id;
            public ulong index;
            public static _StructTransactionInput Encode(TransactionInput structArg) {
                return new _StructTransactionInput {
                    id = _AllocSlice<byte, byte>(structArg.id, 1, 1, _arg35 => _arg35),
                    index = structArg.index
                };
            }
            public TransactionInput Decode() {
                return new TransactionInput {
                    id = _FreeSlice<byte, byte, List<byte>>(this.id, 1, 1, _arg36 => _arg36),
                    index = this.index
                };
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct _StructDatum {
            public byte datumType;
            public _RawTuple2 data;
            public static _StructDatum Encode(Datum structArg) {
                return new _StructDatum {
                    datumType = structArg.datumType,
                    data = _EncodeOption(structArg.data, _arg37 => _AllocSlice<byte, byte>(_arg37, 1, 1, _arg38 => _arg38))
                };
            }
            public Datum Decode() {
                return new Datum {
                    datumType = this.datumType,
                    data = _DecodeOption(this.data, _arg39 => _FreeSlice<byte, byte, List<byte>>(_arg39, 1, 1, _arg40 => _arg40))
                };
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct _StructTransactionOutput {
            public _RawSlice address;
            public _StructValue amount;
            public UIntPtr index;
            public _RawTuple3 datum;
            public _RawSlice raw;
            public static _StructTransactionOutput Encode(TransactionOutput structArg) {
                return new _StructTransactionOutput {
                    address = _AllocSlice<byte, byte>(structArg.address, 1, 1, _arg41 => _arg41),
                    amount = _StructValue.Encode(structArg.amount),
                    index = structArg.index,
                    datum = _EncodeOption(structArg.datum, _arg42 => _StructDatum.Encode(_arg42)),
                    raw = _AllocSlice<byte, byte>(structArg.raw, 1, 1, _arg43 => _arg43)
                };
            }
            public TransactionOutput Decode() {
                return new TransactionOutput {
                    address = _FreeSlice<byte, byte, List<byte>>(this.address, 1, 1, _arg44 => _arg44),
                    amount = (this.amount).Decode(),
                    index = this.index,
                    datum = _DecodeOption(this.datum, _arg45 => (_arg45).Decode()),
                    raw = _FreeSlice<byte, byte, List<byte>>(this.raw, 1, 1, _arg46 => _arg46)
                };
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct _StructValue {
            public ulong coin;
            public _RawSlice multiAsset;
            public static _StructValue Encode(Value structArg) {
                return new _StructValue {
                    coin = structArg.coin,
                    multiAsset = _AllocDict<List<byte>, Dictionary<List<byte>,ulong>, _RawTuple0>(structArg.multiAsset, 32, 8, _arg47 => ((Func<(List<byte>,Dictionary<List<byte>,ulong>), _RawTuple0>)(_arg48 => new _RawTuple0 { elem0 = _AllocSlice<byte, byte>(_arg48.Item1, 1, 1, _arg49 => _arg49),elem1 = _AllocDict<List<byte>, ulong, _RawTuple4>(_arg48.Item2, 24, 8, _arg50 => ((Func<(List<byte>,ulong), _RawTuple4>)(_arg51 => new _RawTuple4 { elem0 = _AllocSlice<byte, byte>(_arg51.Item1, 1, 1, _arg52 => _arg52),elem1 = _arg51.Item2 }))(_arg50)) }))(_arg47))
                };
            }
            public Value Decode() {
                return new Value {
                    coin = this.coin,
                    multiAsset = _FreeDict<List<byte>, Dictionary<List<byte>,ulong>, _RawTuple0, Dictionary<List<byte>, Dictionary<List<byte>,ulong>>>(this.multiAsset, 32, 8, _arg53 => ((Func<_RawTuple0, (List<byte>,Dictionary<List<byte>,ulong>)>)(_arg54 => (_FreeSlice<byte, byte, List<byte>>(_arg54.elem0, 1, 1, _arg55 => _arg55),_FreeDict<List<byte>, ulong, _RawTuple4, Dictionary<List<byte>, ulong>>(_arg54.elem1, 24, 8, _arg56 => ((Func<_RawTuple4, (List<byte>,ulong)>)(_arg57 => (_FreeSlice<byte, byte, List<byte>>(_arg57.elem0, 1, 1, _arg58 => _arg58),_arg57.elem1)))(_arg56)))))(_arg53))
                };
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct _StructRedeemer {
            public byte tag;
            public uint index;
            public _RawSlice data;
            public _StructExUnits exUnits;
            public static _StructRedeemer Encode(Redeemer structArg) {
                return new _StructRedeemer {
                    tag = structArg.tag,
                    index = structArg.index,
                    data = _AllocSlice<byte, byte>(structArg.data, 1, 1, _arg59 => _arg59),
                    exUnits = _StructExUnits.Encode(structArg.exUnits)
                };
            }
            public Redeemer Decode() {
                return new Redeemer {
                    tag = this.tag,
                    index = this.index,
                    data = _FreeSlice<byte, byte, List<byte>>(this.data, 1, 1, _arg60 => _arg60),
                    exUnits = (this.exUnits).Decode()
                };
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct _StructExUnits {
            public ulong mem;
            public ulong steps;
            public static _StructExUnits Encode(ExUnits structArg) {
                return new _StructExUnits {
                    mem = structArg.mem,
                    steps = structArg.steps
                };
            }
            public ExUnits Decode() {
                return new ExUnits {
                    mem = this.mem,
                    steps = this.steps
                };
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct _StructNextResponse {
            public byte action;
            public _RawTuple5 tip;
            public _RawTuple5 block;
            public static _StructNextResponse Encode(NextResponse structArg) {
                return new _StructNextResponse {
                    action = structArg.action,
                    tip = _EncodeOption(structArg.tip, _arg61 => _StructBlock.Encode(_arg61)),
                    block = _EncodeOption(structArg.block, _arg62 => _StructBlock.Encode(_arg62))
                };
            }
            public NextResponse Decode() {
                return new NextResponse {
                    action = this.action,
                    tip = _DecodeOption(this.tip, _arg63 => (_arg63).Decode()),
                    block = _DecodeOption(this.block, _arg64 => (_arg64).Decode())
                };
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct _StructNodeClientWrapper {
            public UIntPtr clientDataPtr;
            public static _StructNodeClientWrapper Encode(NodeClientWrapper structArg) {
                return new _StructNodeClientWrapper {
                    clientDataPtr = structArg.clientDataPtr
                };
            }
            public NodeClientWrapper Decode() {
                return new NodeClientWrapper {
                    clientDataPtr = this.clientDataPtr
                };
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct _StructPallasUtility {
            public static _StructPallasUtility Encode(PallasUtility structArg) {
                return new _StructPallasUtility {
                };
            }
            public PallasUtility Decode() {
                return new PallasUtility {
                };
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct _StructTxSubmit {
            public static _StructTxSubmit Encode(TxSubmit structArg) {
                return new _StructTxSubmit {
                };
            }
            public TxSubmit Decode() {
                return new TxSubmit {
                };
            }
        }
        [DllImport("pallas_dotnet_rs", EntryPoint = "rnet_export_mainnet_magic", CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong _FnMainnetMagic(
        );
        [DllImport("pallas_dotnet_rs", EntryPoint = "rnet_export_testnet_magic", CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong _FnTestnetMagic(
        );
        [DllImport("pallas_dotnet_rs", EntryPoint = "rnet_export_preview_magic", CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong _FnPreviewMagic(
        );
        [DllImport("pallas_dotnet_rs", EntryPoint = "rnet_export_pre_production_magic", CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong _FnPreProductionMagic(
        );
        [DllImport("pallas_dotnet_rs", EntryPoint = "rnet_export_connect", CallingConvention = CallingConvention.Cdecl)]
        private static extern _StructNodeClientWrapper _FnConnect(
            _RawSlice socketPath,
            ulong networkMagic
        );
        [DllImport("pallas_dotnet_rs", EntryPoint = "rnet_export_get_utxo_by_address_cbor", CallingConvention = CallingConvention.Cdecl)]
        private static extern _RawSlice _FnGetUtxoByAddressCbor(
            _StructNodeClientWrapper clientWrapper,
            _RawSlice address
        );
        [DllImport("pallas_dotnet_rs", EntryPoint = "rnet_export_get_tip", CallingConvention = CallingConvention.Cdecl)]
        private static extern _StructPoint _FnGetTip(
            _StructNodeClientWrapper clientWrapper
        );
        [DllImport("pallas_dotnet_rs", EntryPoint = "rnet_export_find_intersect", CallingConvention = CallingConvention.Cdecl)]
        private static extern _RawTuple6 _FnFindIntersect(
            _StructNodeClientWrapper clientWrapper,
            _StructPoint knownPoint
        );
        [DllImport("pallas_dotnet_rs", EntryPoint = "rnet_export_chain_sync_next", CallingConvention = CallingConvention.Cdecl)]
        private static extern _StructNextResponse _FnChainSyncNext(
            _StructNodeClientWrapper clientWrapper
        );
        [DllImport("pallas_dotnet_rs", EntryPoint = "rnet_export_chain_sync_has_agency", CallingConvention = CallingConvention.Cdecl)]
        private static extern byte _FnChainSyncHasAgency(
            _StructNodeClientWrapper clientWrapper
        );
        [DllImport("pallas_dotnet_rs", EntryPoint = "rnet_export_disconnect", CallingConvention = CallingConvention.Cdecl)]
        private static extern void _FnDisconnect(
            _StructNodeClientWrapper clientWrapper
        );
        [DllImport("pallas_dotnet_rs", EntryPoint = "rnet_export_address_bytes_to_bech32", CallingConvention = CallingConvention.Cdecl)]
        private static extern _RawSlice _FnAddressBytesToBech32(
            _RawSlice addressBytes
        );
        [DllImport("pallas_dotnet_rs", EntryPoint = "rnet_export_submit_tx", CallingConvention = CallingConvention.Cdecl)]
        private static extern _RawSlice _FnSubmitTx(
            _RawSlice server,
            ulong magic,
            _RawSlice tx
        );
        [StructLayout(LayoutKind.Sequential)]
        private struct _RawTuple0 {
            public _RawSlice elem0;
            public _RawSlice elem1;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct _RawTuple1 {
            public _RawSlice elem0;
            public long elem1;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct _RawTuple2 {
            public _RawSlice elem0;
            public byte elem1;
        }
        private static _RawTuple2 _EncodeOption<T>(T arg, Func<T, _RawSlice> converter) {
            if (arg != null) {
                return new _RawTuple2 { elem0 = converter(arg), elem1 = 1 };
            } else {
                return new _RawTuple2 { elem0 = default(_RawSlice), elem1 = 0 };
            }
        }
        private static T _DecodeOption<T>(_RawTuple2 arg, Func<_RawSlice, T> converter) {
            if (arg.elem1 != 0) {
                return converter(arg.elem0);
            } else {
                return default(T);
            }
        }
        private static _RawTuple2 _EncodeResult(Action f) {
            try {
                f();
                return new _RawTuple2 { elem0 = default(_RawSlice), elem1 = 1 };
            } catch (Exception e) {
                return new _RawTuple2 { elem0 = _AllocStr(e.Message), elem1 = 0 };
            }
        }
        private static void _DecodeResult(_RawTuple2 arg) {
            if (arg.elem1 == 0) {
                throw new RustException(_FreeStr(arg.elem0));
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct _RawTuple3 {
            public _StructDatum elem0;
            public byte elem1;
        }
        private static _RawTuple3 _EncodeOption<T>(T arg, Func<T, _StructDatum> converter) {
            if (arg != null) {
                return new _RawTuple3 { elem0 = converter(arg), elem1 = 1 };
            } else {
                return new _RawTuple3 { elem0 = default(_StructDatum), elem1 = 0 };
            }
        }
        private static T _DecodeOption<T>(_RawTuple3 arg, Func<_StructDatum, T> converter) {
            if (arg.elem1 != 0) {
                return converter(arg.elem0);
            } else {
                return default(T);
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct _RawTuple4 {
            public _RawSlice elem0;
            public ulong elem1;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct _RawTuple5 {
            public _StructBlock elem0;
            public byte elem1;
        }
        private static _RawTuple5 _EncodeOption<T>(T arg, Func<T, _StructBlock> converter) {
            if (arg != null) {
                return new _RawTuple5 { elem0 = converter(arg), elem1 = 1 };
            } else {
                return new _RawTuple5 { elem0 = default(_StructBlock), elem1 = 0 };
            }
        }
        private static T _DecodeOption<T>(_RawTuple5 arg, Func<_StructBlock, T> converter) {
            if (arg.elem1 != 0) {
                return converter(arg.elem0);
            } else {
                return default(T);
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct _RawTuple6 {
            public _StructPoint elem0;
            public byte elem1;
        }
        private static _RawTuple6 _EncodeOption<T>(T arg, Func<T, _StructPoint> converter) {
            if (arg != null) {
                return new _RawTuple6 { elem0 = converter(arg), elem1 = 1 };
            } else {
                return new _RawTuple6 { elem0 = default(_StructPoint), elem1 = 0 };
            }
        }
        private static T _DecodeOption<T>(_RawTuple6 arg, Func<_StructPoint, T> converter) {
            if (arg.elem1 != 0) {
                return converter(arg.elem0);
            } else {
                return default(T);
            }
        }


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void _ManageDelegateDelegate(IntPtr ptr, int adjust);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void _DropOpaqueDelegate(IntPtr ptr);

        private static Dictionary<IntPtr, (int, Delegate, Delegate)> _ActiveDelegates = new Dictionary<IntPtr, (int, Delegate, Delegate)>();

        private static _ManageDelegateDelegate _manageDelegate = _ManageDelegate;
        private static IntPtr _manageDelegatePtr = Marshal.GetFunctionPointerForDelegate(_manageDelegate);

        private static void _ManageDelegate(IntPtr ptr, int adjust)
        {
            lock (_ActiveDelegates)
            {
                var item = _ActiveDelegates[ptr];
                item.Item1 += adjust;
                if (item.Item1 > 0)
                {
                    _ActiveDelegates[ptr] = item;
                }
                else
                {
                    _ActiveDelegates.Remove(ptr);
                }
            }
        }

        private static _RawDelegate _AllocDelegate(Delegate d, Delegate original)
        {
            var ptr = Marshal.GetFunctionPointerForDelegate(d);
            lock (_ActiveDelegates)
            {
                if (_ActiveDelegates.ContainsKey(ptr))
                {
                    var item = _ActiveDelegates[ptr];
                    item.Item1 += 1;
                    _ActiveDelegates[ptr] = item;
                } else
                {
                    _ActiveDelegates.Add(ptr, (1, d, original));
                }
            }
            return new _RawDelegate
            {
                call_fn = ptr,
                drop_fn = _manageDelegatePtr,
            };
        }

        private static Delegate _FreeDelegate(_RawDelegate d)
        {
            var ptr = d.call_fn;
            lock (_ActiveDelegates)
            {
                var item = _ActiveDelegates[ptr];
                item.Item1 -= 1;
                if (item.Item1 > 0)
                {
                    _ActiveDelegates[ptr] = item;
                }
                else
                {
                    _ActiveDelegates.Remove(ptr);
                }
                return item.Item3;
            }
        }

        [DllImport("pallas_dotnet_rs", EntryPoint = "rnet_alloc", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr _Alloc( UIntPtr size, UIntPtr align);

        [DllImport("pallas_dotnet_rs", EntryPoint = "rnet_free", CallingConvention = CallingConvention.Cdecl)]
        private static extern void _Free(IntPtr ptr, UIntPtr size, UIntPtr align);

        [StructLayout(LayoutKind.Sequential)]
        private struct _RawSlice
        {
            public IntPtr ptr;
            public UIntPtr len;

            public static _RawSlice Alloc(UIntPtr len, int size, int align)
            {
                if (len == UIntPtr.Zero)
                {
                    return new _RawSlice {
                        ptr = (IntPtr)align,
                        len = UIntPtr.Zero,
                    };
                } else
                {
                    return new _RawSlice
                    {
                        ptr = _Alloc((UIntPtr)((UInt64)len * (UInt64)size), (UIntPtr)align),
                        len = len,
                    };
                }
            }

            public void Free(int size, int align)
            {
                if (len != UIntPtr.Zero)
                {
                    _Free(ptr, (UIntPtr)((UInt64)len * (UInt64)size), (UIntPtr)align);
                    ptr = (IntPtr)1;
                    len = UIntPtr.Zero;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct _RawOpaqueHandle
        {
            public IntPtr ptr;
            public IntPtr drop_fn;
            public ulong type_id;

            public void Drop()
            {
                if (ptr != IntPtr.Zero)
                {
                    var drop = Marshal.GetDelegateForFunctionPointer<_DropOpaqueDelegate>(drop_fn);
                    drop(ptr);
                    ptr = IntPtr.Zero;
                }
            }
        }

        private class _OpaqueHandle : IOpaqueHandle
        {
            private _RawOpaqueHandle inner;

            public _OpaqueHandle(_RawOpaqueHandle inner)
            {
                this.inner = inner;
            }

            public _RawOpaqueHandle ToInner(ulong type_id)
            {
                if (type_id != inner.type_id)
                {
                    throw new InvalidCastException("Opaque handle does not have the correct type");
                }
                return this.inner;
            }

            ~_OpaqueHandle()
            {
                inner.Drop();
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as _OpaqueHandle);
            }

            public bool Equals(IOpaqueHandle other)
            {
                var casted = other as _OpaqueHandle;
                return casted != null &&
                       inner.ptr == casted.inner.ptr && inner.type_id == casted.inner.type_id;
            }

            public override int GetHashCode()
            {
                return inner.ptr.GetHashCode() + inner.type_id.GetHashCode();
            }

            public void Dispose()
            {
                inner.Drop();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct _RawDelegate
        {
            public IntPtr call_fn;
            public IntPtr drop_fn;
        }

        private static IntPtr _AllocBox<T>(T arg, int size, int align)
        {
            if (size > 0) {
                var ptr = _Alloc((UIntPtr)size, (UIntPtr)align);
                Marshal.StructureToPtr(arg, ptr, false);
                return ptr;
            } else {
                return (IntPtr)align;
            }
        }

        private static _RawSlice _AllocStr(string arg)
        {
            var nb = Encoding.UTF8.GetByteCount(arg);
            var slice = _RawSlice.Alloc((UIntPtr)nb, 1, 1);
            unsafe
            {
                fixed (char* firstChar = arg)
                {
                    nb = Encoding.UTF8.GetBytes(firstChar, arg.Length, (byte*)slice.ptr, nb);
                }
            }
            return slice;
        }

        private static _RawSlice _AllocSlice<T, U>(IReadOnlyCollection<T> collection, int size, int align, Func<T, U> converter) {
            var count = collection.Count;
            var slice = _RawSlice.Alloc((UIntPtr)count, size, align);
            var ptr = slice.ptr;
            foreach (var item in collection) {
                Marshal.StructureToPtr(converter(item), ptr, false);
                ptr = (IntPtr)(ptr.ToInt64() + (long)size);
            }
            return slice;
        }

        private static _RawSlice _AllocDict<TKey, TValue, U>(IReadOnlyDictionary<TKey, TValue> collection, int size, int align, Func<(TKey, TValue), U> converter) where U: unmanaged
        {
            var count = collection.Count;
            var slice = _RawSlice.Alloc((UIntPtr)count, size, align);
            var ptr = slice.ptr;
            foreach (var item in collection)
            {
                Marshal.StructureToPtr<U>(converter((item.Key, item.Value)), ptr, false);
                ptr = (IntPtr)(ptr.ToInt64() + (long)size);
            }
            return slice;
        }

        private static T _FreeBox<T>(IntPtr ptr, int size, int align)
        {
            var res = Marshal.PtrToStructure<T>(ptr);
            if (size > 0) {
                _Free(ptr, (UIntPtr)size, (UIntPtr)align);
            }
            return res;
        }

        private static String _FreeStr(_RawSlice arg)
        {
            unsafe
            {
                var res = Encoding.UTF8.GetString((byte*)arg.ptr, (int)arg.len);
                arg.Free(1, 1);
                return res;
            }
        }

        private static TList _FreeSlice<T, U, TList>(_RawSlice arg, int size, int align, Func<U, T> converter) where TList: ICollection<T>, new()
        {
            unsafe
            {
                var res = new TList();
                var ptr = arg.ptr;
                for (var i = 0; i < (int)arg.len; ++i) {
                    res.Add(converter(Marshal.PtrToStructure<U>(ptr)));
                    ptr = (IntPtr)(ptr.ToInt64() + (long)size);
                }
                arg.Free(size, align);
                return res;
            }
        }

        private static TDict _FreeDict<TKey, TValue, U, TDict>(_RawSlice arg, int size, int align, Func<U, (TKey, TValue)> converter) where U : unmanaged where TDict: IDictionary<TKey, TValue>, new()
        {
            unsafe
            {
                var res = new TDict();
                var ptr = arg.ptr;
                for (var i = 0; i < (int)arg.len; ++i)
                {
                    var item = converter(Marshal.PtrToStructure<U>(ptr));
                    res.Add(item.Item1, item.Item2);
                    ptr = (IntPtr)(ptr.ToInt64() + (long)size);
                }
                arg.Free(size, align);
                return res;
            }
        }
    }
}

