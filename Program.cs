using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;

namespace QuickPrep
{
    class Program
    {
        static void Main(string[] args)
        {
            // 讓視窗一開始乾乾淨淨
            Console.Clear();
            
            // --- 視覺標題 ---
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
    ╔══════════════════════════════════════════════════════╗
    ║                 QUICK PREP v2.5                      ║
    ║         Professional Project Initializer             ║
    ╚══════════════════════════════════════════════════════╝");
            Console.ResetColor();

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
                templates.Add(new ProjectTemplate { Name = "Web 開發", Folders = new[] { "src", "wwwroot", "docs" } });
                string defaultJson = JsonSerializer.Serialize(templates, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(jsonPath, defaultJson);
                PrintSystemMsg("已建立預設 templates.json");
            }

            // --- 2. 詢問專案資訊 ---
            PrintHeader("1. 專案路徑設定");
            
            Console.Write("❯ 專案名稱: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            string projectName = Console.ReadLine() ?? "NewProject";
            Console.ResetColor();

            Console.Write("❯ 目標目錄 (Enter 為當前, 或輸入 'desktop'): ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            string? inputDir = Console.ReadLine();
            Console.ResetColor();

            string targetDir = inputDir?.ToLower() == "desktop" 
                ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop) 
                : (string.IsNullOrWhiteSpace(inputDir) ? Environment.CurrentDirectory : Path.GetFullPath(inputDir));

            string finalProjectPath = Path.Combine(targetDir, projectName);

            // --- 3. 動態顯示模板選單 ---
            PrintHeader("2. 選擇開發模板");
            for (int i = 0; i < templates.Count; i++)
            {
                // 使用 PadRight 讓文字對齊，看起來更整齊
                string folderPreview = string.Join(", ", templates[i].Folders.Take(2));
                Console.WriteLine($"  [{i + 1}] {templates[i].Name.PadRight(20)} │ (範例: {folderPreview}...)");
            }
            Console.WriteLine($"  [{templates.Count + 1}] 手動自定義專案");
            
            Console.Write($"\n❯ 請輸入編號 (1-{templates.Count + 1}): ");
            string choice = Console.ReadLine() ?? "";
            List<string> foldersToCreate = new List<string>();
            string finalTemplateName = "手動自定義";

            if (int.TryParse(choice, out int index) && index >= 1 && index <= templates.Count)
            {
                foldersToCreate.AddRange(templates[index - 1].Folders);
                finalTemplateName = templates[index - 1].Name;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✔ 已載入模板: {finalTemplateName}");
                Console.ResetColor();
            }

            // --- 4. 手動補充邏輯 ---
            PrintHeader("3. 補充資料夾結構");
            Console.WriteLine("提示: 輸入路徑 (如 src/utils) 或按 'done' 結束。");
            
            while (true)
            {
                Console.Write("  + 追加路徑 > ");
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "done") break;
                foldersToCreate.Add(input.Trim());
            }

            // --- 5. 執行建立 ---
            CreateProjectStructure(finalProjectPath, foldersToCreate.ToArray(), finalTemplateName);
        }

        // --- 輔助美化方法 ---
        static void PrintHeader(string title)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"─── {title} ─────────────────────────────────");
            Console.ResetColor();
        }

        static void PrintSystemMsg(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"[系統] {msg}");
            Console.ResetColor();
        }

        static void CreateProjectStructure(string root, string[] folders, string templateName)
        {
            PrintHeader("4. 正在建構專案...");
            Console.WriteLine($"📍 目標路徑: {root}\n");
            
            Directory.CreateDirectory(root);

            var sortedFolders = folders.OrderBy(f => f).ToArray();
            foreach (string folder in sortedFolders)
            {
                string fullPath = Path.Combine(root, folder);
                Directory.CreateDirectory(fullPath);
                File.WriteAllText(Path.Combine(fullPath, ".gitkeep"), "");

                // 美化輸出的樹狀符號
                string prettyPath = folder.Replace("/", " ── ").Replace("\\", " ── ");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("  [DIR] ");
                Console.ResetColor();
                Console.WriteLine(prettyPath);
            }

            GenerateReadme(root, templateName, sortedFolders);
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n⭐ 專案初始化成功！");
            Console.WriteLine("--------------------------------------------------");
            Console.ResetColor();
            Console.WriteLine("現在你可以開啟 VS Code 或其他編輯器開始開發了。\n");
        }

        static void GenerateReadme(string root, string templateName, string[] folders)
        {
            string filePath = Path.Combine(root, "README.md");
            string projectName = Path.GetFileName(root);
            
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"# {projectName}");
            sb.AppendLine($"\n> 本專案由 **QuickPrep** 自動初始化於 {DateTime.Now:yyyy-MM-dd HH:mm}。");
            sb.AppendLine($"\n## 📋 專案模板: {templateName}");
            sb.AppendLine("\n### 📂 目錄結構");
            sb.AppendLine("```text");
            foreach (var folder in folders) sb.AppendLine(folder);
            sb.AppendLine("```");
            sb.AppendLine("\n## 🚀 快速開始\n1. 確認環境已就緒。\n2. 於 `src` 展開開發。");
            sb.AppendLine("\n---\n*Generated by QuickPrep*");

            File.WriteAllText(filePath, sb.ToString());
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("  [FILE] README.md (已產生)");
            Console.ResetColor();
        }
    }
}

public class ProjectTemplate
{
    public string Name { get; set; } = "";
    public string[] Folders { get; set; } = Array.Empty<string>();
}