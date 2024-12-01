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
            var worksheet = package.Workbook.Worksheets[0]; // 获取第一个工作表
            int rowCount = worksheet.Dimension.Rows; // 获取行数
            int colCount = worksheet.Dimension.Columns; // 获取列数
            Debug.Log("Row Count: " + rowCount);

            for (int row = 2; row <= rowCount; row++) // 从第二行开始读取 第一行是表头
            {
                string descript = worksheet.Cells[row, 3].Text; // 第3列: 列名
                string fieldName = worksheet.Cells[row, 1].Text; // 第1列：字段名
                string type = worksheet.Cells[row, 2].Text; // 第2列：类型
                string value = worksheet.Cells[row, 4].Text; // 第4列：值
                value = ConvertValueByType(value, type); // 根据类型转换值
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

    // 根据类型字符串来转换值
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