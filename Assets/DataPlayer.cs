using UnityEngine;

public class DataPlayer : MonoBehaviour
{
    public PlayerData playerData = new PlayerData();

    // Метод для установки номера локации для загрузки
    public void SetSceneToLoad(int sceneNumber)
    {
        playerData.sceneToLoad = sceneNumber;
    }

    // Метод для увеличения размера массива на один элемент
    public void IncreaseArraySize()
    {
        playerData.additionalArray = ResizeArray(playerData.additionalArray, playerData.additionalArray.Length + 1);
    }

    // Вспомогательный метод для изменения размера массива
    private bool[] ResizeArray(bool[] array, int newSize)
    {
        bool[] newArray = new bool[newSize];
        for (int i = 0; i < Mathf.Min(array.Length, newSize); i++)
        {
            newArray[i] = array[i];
        }
        return newArray;
    }
}

[System.Serializable]
public class PlayerData
{
    public int id;
    public int health;
    public bool isAlive;
    public int sceneToLoad;
    public bool[] additionalArray = new bool[5] { false, false, false, false, false }; // Изначально массив содержит 5 элементов
}
