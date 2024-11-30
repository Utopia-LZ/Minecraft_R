using System.IO;
using System.Text;
using UnityEditor;
using OfficeOpenXml;
using UnityEngine;

public class Excel2Json
{
    public static string filePath = "C:\\Users\\33572\\Desktop\\Minecraft_R\\Src\\Config\\config.xlsx";
    public static string outputPath = "C:\\Users\\33572\\Desktop\\Minecraft_R\\Src\\Config\\config.txt";
    public static string serverPath = "C:\\Users\\33572\\Desktop\\Minecraft_R\\Server\\Data\\config.txt";

    [MenuItem("Tools/Excel2Json")]
    public static void Convert()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{\n");
        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            var worksheet = package.Workbook.Worksheets[0]; // 获取第一个工作表
            int rowCount = worksheet.Dimension.Rows; // 获取行数
            int colCount = worksheet.Dimension.Columns; // 获取列数
            Debug.Log("Row Count: " + rowCount);

            for (int row = 2; row <= rowCount; row++) // 从第二行开始读取 第一行是表头
            {
                string fieldName = worksheet.Cells[row,2].Text; // 第2列：字段名
                string type = worksheet.Cells[row, 3].Text; // 第3列：类型
                string value = worksheet.Cells[row, 4].Text; // 第4列：值
                value = ConvertValueByType(value, type); // 根据类型转换值
                sb.Append($"\t\"{fieldName}\" : {value}");
                if (row != rowCount) sb.Append(",\n");
            }
        }
        sb.Append("\n}");
        StreamWriter sw = new StreamWriter(outputPath);
        sw.Write(sb.ToString());
        sw.Close();
        sw = new StreamWriter(serverPath);
        sw.Write(sb.ToString());
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