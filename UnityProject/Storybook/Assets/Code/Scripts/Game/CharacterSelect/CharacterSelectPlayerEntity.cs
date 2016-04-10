using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class CharacterSelectPlayerEntity : PlayerObject
{
    private Genre m_genre;

    [SyncProperty]
    public Genre PlayerGenre
    {
        get { return m_genre; }
        set
        {
            Assert.IsTrue(ShouldBeChanging);
            m_genre = value;
            PropertyChanged();
        }
    }
}
