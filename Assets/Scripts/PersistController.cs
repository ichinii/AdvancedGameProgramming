using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PersistController : Singleton<PersistController>
{
    [SerializeField] private bool m_isLoaded = false;
    [SerializeField] private int m_recentLevel = -1;
    [SerializeField] private const string m_fileName = "save_game";
    [SerializeField] private string m_filePath;

    public int RecentLevel {
        get { 
            return m_recentLevel;
        }
        set {
            // save the recent level in any case
            if (m_recentLevel != value) {
                m_recentLevel = value;
                Save();
            }
        }
    }

    void Awake()
    {
        m_filePath = Application.persistentDataPath + "/" + m_fileName;
        Load();
    }

    void Load()
    {
        if (File.Exists(m_filePath)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(m_filePath, FileMode.Open);
            if (file != null) {
                m_recentLevel = (int)formatter.Deserialize(file);
                m_isLoaded = true;
            }
        }
    }

    void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Open(m_filePath, FileMode.Create);
        formatter.Serialize(file, m_recentLevel);
    }
}
