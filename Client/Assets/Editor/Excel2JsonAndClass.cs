using System.IO;
using System.Text;
using UnityEditor;
using OfficeOpenXml;
using UnityEngine;
using System;

public class Excel2JsonAndClass
{
    public static string filePath = "C:/Users/33572/Desktop/Minecraft_R/Src/Config/config.xlsx";
    public static string outputPath = "C:/Users/33572/Desktop/Minecraft_R/Src/Config/config.txt";
    public static string serverPath = "C:/Users/33572/Desktop/Minecraft_R/Server/Data/config.txt";
    public static string clientClassPath = "C:/Users/33572/Desktop/Minecraft_R/Client/Assets/Scripts/Framework/Config.cs";
    public static string serverClassPath = "C:/Users/33572/Desktop/Minecraft_R/Server/Scripts/Data/Config.cs";

    [MenuItem("Tools/Excel2JsonAndClass")]
    public static void Convert()
    {
        StringBuilder sbConfig = new StringBuilder();
        StringBuilder sbClass = new StringBuilder();
        sbConfig.Append("{\n");
        sbClass.Append("[System.Serializable]\npublic class Config\n{\n");
        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            var worksheet = package.Workbook.Worksheets[0]; // ��ȡ��һ��������
            int rowCount = worksheet.Dimension.Rows; // ��ȡ����
            int colCount = worksheet.Dimension.Columns; // ��ȡ����
            Debug.Log("Row Count: " + rowCount);

            for (int row = 2; row <= rowCount; row++) // �ӵڶ��п�ʼ��ȡ ��һ���Ǳ�ͷ
            {
                string descript = worksheet.Cells[row, 3].Text; // ��3��: ����
                string fieldName = worksheet.Cells[row, 1].Text; // ��1�У��ֶ���
                string type = worksheet.Cells[row, 2].Text; // ��2�У�����
                string value = worksheet.Cells[row, 4].Text; // ��4�У�ֵ
                value = ConvertValueByType(value, type); // ��������ת��ֵ
                sbConfig.Append($"\t\"{fieldName}\" : {value}");
                if (row != rowCount) sbConfig.Append(",\n");
                sbClass.Append($"\tpublic {type} {fieldName}; //{descript}\n");
            }
        }
        sbConfig.Append("\n}");
        sbClass.Append("}");
        StreamWriter sw = new StreamWriter(outputPath);
        sw.Write(sbConfig.ToString());
        sw.Close();
        sw = new StreamWriter(serverPath);
        sw.Write(sbConfig.ToString());
        sw.Close();
        sw = new StreamWriter(clientClassPath);
        sw.Write(sbClass.ToString());
        sw.Close();
        sw = new StreamWriter(serverClassPath);
        sw.Write(sbClass.ToString());
        sw.Close();

        Debug.Log("Done.");
    }

    // ���������ַ�����ת��ֵ
    static string ConvertValueByType(string value, string type)
    {
        return type.ToLower() switch
        {
            "int" or "float" or "double" or "long" => value,
            "bool" => value.ToLower(),
            "string" => $"\"{value}\"",
            _ => value,
        };
    }
}