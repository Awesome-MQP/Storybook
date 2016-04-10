using UnityEngine;
using System.Collections;
using System.Linq;
using Photon;
using UnityEngine.Assertions;

[RequireComponent(typeof(PhotonView))]
public class TestSync : PunBehaviour
{
    [SerializeField]
    private TestSync m_other;

    [SyncProperty]
    public TestSync Other
    {
        get { return m_other; }
        set
        {
            Assert.IsTrue(ShouldBeChanging);
            m_other = value;
            PropertyChanged();
        }
    }

    public override void OnStartOwner(bool wasSpawn)
    {
        StartCoroutine(WaitToSet());
    }

    private IEnumerator WaitToSet()
    {
        yield return new WaitForSeconds(10.0f);

        TestSync[] syncObjects = FindObjectsOfType<TestSync>();
        Other = syncObjects.First(sync => sync != this);
    }
}
