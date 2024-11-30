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
            var worksheet = package.Workbook.Worksheets[0]; // ��ȡ��һ��������
            int rowCount = worksheet.Dimension.Rows; // ��ȡ����
            int colCount = worksheet.Dimension.Columns; // ��ȡ����
            Debug.Log("Row Count: " + rowCount);

            for (int row = 2; row <= rowCount; row++) // �ӵڶ��п�ʼ��ȡ ��һ���Ǳ�ͷ
            {
                string fieldName = worksheet.Cells[row,2].Text; // ��2�У��ֶ���
                string type = worksheet.Cells[row, 3].Text; // ��3�У�����
                string value = worksheet.Cells[row, 4].Text; // ��4�У�ֵ
                value = ConvertValueByType(value, type); // ��������ת��ֵ
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