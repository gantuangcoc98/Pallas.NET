using PallasDotnet;
using PallasDotnet.Models;

namespace pallas_dotnet_test;

public class UnitTest1
{
    [Fact]
    public void FindIntersectTest()
    {
        var nodeClientWrapper = PallasDotnetRs.PallasDotnetRs.Connect("/tmp/node.socket", 2);
        var point = PallasDotnetRs.PallasDotnetRs.FindIntersect(nodeClientWrapper, new (){
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
    public void ChainSyncTest() {
        var nodeClientWrapper = PallasDotnetRs.PallasDotnetRs.Connect("/tmp/node.socket", 2);
        var point = PallasDotnetRs.PallasDotnetRs.GetTip(nodeClientWrapper);
        var intersect = PallasDotnetRs.PallasDotnetRs.FindIntersect(nodeClientWrapper, new (){
            slot = point.slot,
            hash = point.hash
        }); 
        var rollbackResponse = PallasDotnetRs.PallasDotnetRs.ChainSyncNext(nodeClientWrapper);
        var rollforwardResponse = PallasDotnetRs.PallasDotnetRs.ChainSyncNext(nodeClientWrapper);

        Assert.True((NextResponseAction)rollbackResponse.action == NextResponseAction.RollBack);
        Assert.True((NextResponseAction)rollforwardResponse.action == NextResponseAction.RollForward);
    }
}