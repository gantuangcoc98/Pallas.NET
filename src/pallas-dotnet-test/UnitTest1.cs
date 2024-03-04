using PallasDotnet;
using PallasDotnet.Models;

namespace pallas_dotnet_test;

public class UnitTest1
{
    [Fact]
    public void FindIntersectTest()
    {
        var nodeClientWrapper = PallasDotnetRs.PallasDotnetRs.Connect("/tmp/node.socket", 2);
        var point = PallasDotnetRs.PallasDotnetRs.FindIntersect(nodeClientWrapper, new()
        {
            slot = 35197575,
            hash = [.. Convert.FromHexString("a9e99c93352f91233a61fb55da83a43c49abf1c84a636e226e11be5ac0343dc3")]
        });

        Assert.True(point.slot == 35197575);
    }

    [Fact]
    public void GetTipTest()
    {
        var nodeClientWrapper = PallasDotnetRs.PallasDotnetRs.Connect("/tmp/node.socket", 2);
        var point = PallasDotnetRs.PallasDotnetRs.GetTip(nodeClientWrapper);
        Assert.True(point.slot > 0);
    }

    [Fact]
    public async void ChainSyncTest()
    {
        var nodeClientWrapper = PallasDotnetRs.PallasDotnetRs.Connect("/tmp/node.socket", 2);
        var point = PallasDotnetRs.PallasDotnetRs.GetTip(nodeClientWrapper);
        var intersect = PallasDotnetRs.PallasDotnetRs.FindIntersect(nodeClientWrapper, new()
        {
            slot = point.slot,
            hash = point.hash
        });
        // Rollback is always the first next response 
        PallasDotnetRs.PallasDotnetRs.ChainSyncNext(nodeClientWrapper);

        var count = 0;
        while (true)
        {
            if (PallasDotnetRs.PallasDotnetRs.ChainSyncHasAgency(nodeClientWrapper))
            {
                var rollforwardResponse = PallasDotnetRs.PallasDotnetRs.ChainSyncNext(nodeClientWrapper);
                if ((NextResponseAction)rollforwardResponse.action == NextResponseAction.RollForward)
                {
                    count++;
                }
            }

            if (count >= 1)
            {
                break;
            }

            await Task.Delay(1000);
        }
    }

    [Fact]
    public void SubmiTxTest()
    {
        var nodeClientWrapper = PallasDotnetRs.PallasDotnetRs.Connect("/tmp/node.socket", 2);
        var result = PallasDotnetRs.PallasDotnetRs.SubmitTx(nodeClientWrapper, Convert.FromHexString("84A300828258207833C101F6A0F80F8BFDD6D90184CAB2A551CE9A595C0D4635E403FFDC31EA55018258207833C101F6A0F80F8BFDD6D90184CAB2A551CE9A595C0D4635E403FFDC31EA55020183A300581D7035FEB70954E97F2601B84C734D2A5315EE0A5E321E4B4F0E489C4CB201821A004C4B40A1581C8B05E87A51C1D4A0FA888D2BB14DBC25E8C343EA379A171B63AA84A0A144434E43541A000186A0028201D81858CBD8799FD8799F581CE63022B0F461602484968BB10FD8F872787B862ACE2D7E943292A370FFD8799FD8799FD8799F581CE63022B0F461602484968BB10FD8F872787B862ACE2D7E943292A370FFD8799FD8799FD8799F581C03EC6A12860EF8C07D4C1A8DE7DF06ACB0F0330A6087ECBE972082A7FFFFFFFFD8799FFFFF1A000493E0D8799F051864FF581C8B05E87A51C1D4A0FA888D2BB14DBC25E8C343EA379A171B63AA84A044434E4354581C5496B3318F8CA933BBFDF19B8FAA7F948D044208E0278D62C24EE73EFF82583900E63022B0F461602484968BB10FD8F872787B862ACE2D7E943292A37003EC6A12860EF8C07D4C1A8DE7DF06ACB0F0330A6087ECBE972082A7821A0011A008A1581C8B05E87A51C1D4A0FA888D2BB14DBC25E8C343EA379A171B63AA84A0A144434E43541A00030D4082583900E63022B0F461602484968BB10FD8F872787B862ACE2D7E943292A37003EC6A12860EF8C07D4C1A8DE7DF06ACB0F0330A6087ECBE972082A71B00000004A345246C021A0002D039A100818258206395DDF277DCCA535AAB1C5C65680E8C73AAE3AB883060EF88C45A2DDE1287325840653EB36C0C6019599F7E8376BC458C5C8774BEC66BE951D09BC4009753C58A297E6D8EA5318CBFEC6F1C11E2A88F234B2F467E64C7D089D350E5CFCCA9E56401F5F6"));
        Assert.True(result);
    }
}