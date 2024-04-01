<<<<<<< HEAD
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDataPersistence : MonoBehaviour
{

=======
public interface IDataPersistence
{
    public void LoadData(GameData data);
    public void SaveData(ref GameData data);
>>>>>>> 13d9ebf8a77db3d10e3b3126d7dd30b089350ee8
}
