using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO; //System.IO.FileInfo, System.IO.StreamReader, System.IO.StreamWriter
using System; //Exception
using System.Text;

public static class FileImport
{
    public static List<string[]> ReadCSV(string filename)
    {
        List<string[]> tempList = new List<string[]>();

        FileInfo fi = new FileInfo(Application.dataPath + "/ImportData/" + filename + ".csv");
        try
        {
            StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8);

            while (sr.Peek() != -1)
            {
                string line = sr.ReadLine();
                tempList.Add(line.Split(','));
                // Debug.Log(line);
            }
        }
        catch (Exception e)
        {
            string[] err = { "新春シャンソンショー" };
            tempList.Add(err);
        }

        return tempList;

        /*List<string[]> tempList = new List<string[]>();

        TextAsset csvFile;
        csvFile = Resources.Load(filename) as TextAsset; // Resouces下のCSV読み込み
        // Debug.Log(csvFile.text);
        StringReader reader = new StringReader(csvFile.text);

        // , で分割しつつ一行ずつ読み込み
        // リストに追加していく
        while (reader.Peek() != -1) // reader.Peaekが-1になるまで
        {
            string line = reader.ReadLine(); // 一行ずつ読み込み
            tempList.Add(line.Split(',')); // , 区切りでリストに追加
        }

        // tempList[行][列]を指定して値を自由に取り出せる
        
        return tempList;*/
    }
}
