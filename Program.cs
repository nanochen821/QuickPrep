using System;
using System.IO;
using System.Linq;
using System.Collections.Generic; // 為了使用 List
using System.Text.Json;           // 為了解析 JSON

namespace QuickPrep
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== QuickPrep 專案初始化工具 v2.0 (JSON 版) ===");

            // --- 1. 讀取模板資料 ---
            string jsonPath = "templates.json";
            List<ProjectTemplate> templates = new List<ProjectTemplate>();

            if (File.Exists(jsonPath))
            {
                string jsonString = File.ReadAllText(jsonPath);
                templates = JsonSerializer.Deserialize<List<ProjectTemplate>>(jsonString) ?? new List<ProjectTemplate>();
            }
            else
            {
                // 如果檔案不存在，建立一個預設的範例給使用者參考
                templates.Add(new ProjectTemplate { Name = "Web 開發", Folders = new[] { "src", "wwwroot", "docs" } });
                string defaultJson = JsonSerializer.Serialize(templates, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(jsonPath, defaultJson);
                Console.WriteLine("[系統] 已為您建立預設 templates.json，您可以自行修改。");
            }

            // --- 2. 詢問專案資訊 (沿用你的路徑邏輯) ---
            Console.Write("請輸入專案名稱: ");
            string projectName = Console.ReadLine() ?? "NewProject";

            Console.Write("請輸入目標目錄 (直接按 Enter 代表當前目錄，或輸入 'desktop'): ");
            string? inputDir = Console.ReadLine();
            string targetDir = inputDir?.ToLower() == "desktop" 
                ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop) 
                : (string.IsNullOrWhiteSpace(inputDir) ? Environment.CurrentDirectory : Path.GetFullPath(inputDir));

            string finalProjectPath = Path.Combine(targetDir, projectName);

            // --- 3. 動態顯示模板選單 ---
            Console.WriteLine("\n請選擇模板類型:");
            for (int i = 0; i < templates.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] {templates[i].Name}");
            }
            Console.WriteLine($"[{templates.Count + 1}] 完全手動自定義");
            Console.Write($"請輸入編號 (1-{templates.Count + 1}): ");

            string choice = Console.ReadLine() ?? "";
            List<string> foldersToCreate = new List<string>();
            string finalTemplateName = "手動自定義"; // 預設一個名稱

            if (int.TryParse(choice, out int index) && index >= 1 && index <= templates.Count)
            {
                // 使用者選了某個 JSON 模板
                foldersToCreate.AddRange(templates[index - 1].Folders);
                finalTemplateName = templates[index - 1].Name; // 更新名稱
                Console.WriteLine($"已載入模板: {finalTemplateName}");
            }

            // --- 4. 手動補充邏輯 (你的第 3 個願望) ---
            Console.WriteLine("\n--- 補充資料夾 (手動模式) ---");
            Console.WriteLine("請輸入額外想建立的路徑 (例如: src/utils)，輸入 'done' 或直接 Enter 結束：");
            
            while (true)
            {
                Console.Write("追加路徑 > ");
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "done") break;
                foldersToCreate.Add(input.Trim());
            }

            // --- 5. 執行建立 ---
            CreateProjectStructure(finalProjectPath, foldersToCreate.ToArray(), finalTemplateName);
        }

        static void CreateProjectStructure(string root, string[] folders, string templateName)
        {
            Console.WriteLine($"\n[開始建立專案: {root}]");
            Directory.CreateDirectory(root);

            var sortedFolders = folders.OrderBy(f => f).ToArray();
            foreach (string folder in sortedFolders)
            {
                string fullPath = Path.Combine(root, folder);
                Directory.CreateDirectory(fullPath);
                
                // 專業細節：在每個子資料夾放入 .gitkeep 確保 Git 追蹤
                File.WriteAllText(Path.Combine(fullPath, ".gitkeep"), "");

                string prettyPath = folder.Replace("/", " └─ ");
                Console.WriteLine($"  {prettyPath}");
            }

            // 呼叫產生 README 的功能
            GenerateReadme(root, templateName, sortedFolders);
            
            Console.WriteLine("\n✅ 專案結構與專業 README 已成功建立！");
        }

        static void GenerateReadme(string root, string templateName, string[] folders)
        {
            string filePath = Path.Combine(root, "README.md");
            string projectName = Path.GetFileName(root);
            
            // 建立專業內容
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"# {projectName}");
            sb.AppendLine($"\n> 本專案由 **QuickPrep** 自動初始化於 {DateTime.Now:yyyy-MM-dd HH:mm}。");
            sb.AppendLine($"\n## 📋 專案模板: {templateName}");
            sb.AppendLine("\n### 📂 目錄結構");
            sb.AppendLine("```text");
            foreach (var folder in folders)
            {
                sb.AppendLine(folder);
            }
            sb.AppendLine("```");
            sb.AppendLine("\n## 🚀 快速開始\n1. 確認開發環境已安裝相關依賴。\n2. 於 `src` 資料夾開始編寫代碼。");
            sb.AppendLine("\n---\n*本文件由 QuickPrep 工具自動生成。*");

            File.WriteAllText(filePath, sb.ToString());
            Console.WriteLine("  [產出] README.md");
        }
    }
}

public class ProjectTemplate
{
    public string Name { get; set; } = "";
    public string[] Folders { get; set; } = Array.Empty<string>();
}

