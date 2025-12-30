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
            // 設定支援 UTF8 編碼以顯示特殊符號
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Clear();

            // --- 程式標題介面 ---
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
    ╔══════════════════════════════════════════════════════╗
    ║                 QUICK PREP v2.8                      ║
    ║         Professional Project Initializer             ║
    ╚══════════════════════════════════════════════════════╝");
            Console.ResetColor();

            // --- 1. 模板資料加載 (支援多路徑讀取與合併) ---
            List<ProjectTemplate> templates = LoadTemplates();

            // --- 2. 專案基本設定 ---
            PrintHeader("1. 專案路徑設定");
            
            Console.Write("❯ 專案名稱: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            string projectName = Console.ReadLine() ?? "NewProject";
            Console.ResetColor();

            Console.Write("❯ 目標目錄 (Enter 為當前路徑, 或輸入 'desktop'): ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            string? inputDir = Console.ReadLine();
            Console.ResetColor();

            string targetDir = inputDir?.ToLower() == "desktop" 
                ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop) 
                : (string.IsNullOrWhiteSpace(inputDir) ? Environment.CurrentDirectory : Path.GetFullPath(inputDir));

            string finalProjectPath = Path.Combine(targetDir, projectName);

            // --- 3. 模板選單顯示 ---
            PrintHeader("2. 選擇開發模板");
            for (int i = 0; i < templates.Count; i++)
            {
                string folderPreview = string.Join(", ", templates[i].Folders.Take(2));
                Console.WriteLine($"  [{i + 1}] {templates[i].Name.PadRight(25)} │ (預覽: {folderPreview}...)");
            }
            Console.WriteLine($"  [{templates.Count + 1}] 手動自定義結構");
            
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

            // --- 4. 擴充結構輸入 (支援空格與大括號語法) ---
            PrintHeader("3. 追加資料夾結構");
            Console.WriteLine("提示: 支援空格分隔 (如: src docs) 或 大括號擴展 (如: assets/{css,js})");
            
            while (true)
            {
                Console.Write("  + 追加路徑 (輸入 'done' 結束) > ");
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "done") break;

                string[] segments = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var seg in segments)
                {
                    if (seg.Contains("{") && seg.Contains("}"))
                        foldersToCreate.AddRange(ExpandBraces(seg));
                    else
                        foldersToCreate.Add(seg.Trim());
                }
            }

            // --- 5. 最終確認與預覽 ---
            PrintHeader("4. 確認建構清單");
            var finalFolders = foldersToCreate.Distinct().OrderBy(f => f).ToList();
            
            if (finalFolders.Count > 0)
            {
                foreach (var folder in finalFolders)
                    Console.WriteLine($"  [待建立] {folder}");
                
                Console.WriteLine($"\n總計: {finalFolders.Count} 個資料夾。");
                Console.Write("\n確認請按 Enter 開始執行 (或按 Ctrl+C 放棄): ");
                Console.ReadLine();
                
                // --- 6. 執行建立程序 ---
                CreateProjectStructure(finalProjectPath, finalFolders.ToArray(), finalTemplateName);
            }
            else
            {
                Console.WriteLine("  ⚠️ 未選取任何結構，操作已取消。");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n程序執行完畢，按任意鍵關閉視窗...");
            Console.ResetColor();
            Console.ReadKey();
        }

        // 讀取多個來源的模板並合併
        static List<ProjectTemplate> LoadTemplates()
        {
            List<ProjectTemplate> templates = new List<ProjectTemplate>();
            string exeDir = AppContext.BaseDirectory;
            string currentDir = Environment.CurrentDirectory;

            string[] paths = { Path.Combine(exeDir, "templates.json"), Path.Combine(currentDir, "templates.json") };

            foreach (string path in paths.Distinct())
            {
                if (File.Exists(path))
                {
                    try
                    {
                        string json = File.ReadAllText(path);
                        var loaded = JsonSerializer.Deserialize<List<ProjectTemplate>>(json);
                        if (loaded != null)
                        {
                            foreach (var t in loaded)
                            {
                                if (!templates.Any(e => e.Name == t.Name)) templates.Add(t);
                            }
                        }
                    }
                    catch (Exception ex) { Console.WriteLine($"[系統] 無法讀取 {path}: {ex.Message}"); }
                }
            }

            // 內建保險模板
            if (templates.Count == 0)
            {
                templates.Add(new ProjectTemplate { Name = "Modern Web Frontend", Folders = new[] { "src/components", "src/assets", "public" } });
                templates.Add(new ProjectTemplate { Name = "Python Data Science", Folders = new[] { "data", "notebooks", "src" } });
            }
            return templates;
        }

        // 解析大括號語法: assets/{css,js} -> assets/css, assets/js
        static List<string> ExpandBraces(string input)
        {
            var results = new List<string>();
            try 
            {
                int start = input.IndexOf('{');
                int end = input.IndexOf('}');
                if (start == -1 || end == -1 || end < start) { results.Add(input); return results; }

                string prefix = input.Substring(0, start);
                string suffix = input.Substring(end + 1);
                string[] parts = input.Substring(start + 1, end - start - 1).Split(',');

                foreach (var p in parts) results.Add($"{prefix}{p.Trim()}{suffix}");
            }
            catch { results.Add(input); }
            return results;
        }

        static void PrintHeader(string title)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"─── {title} ─────────────────────────────────");
            Console.ResetColor();
        }

        static void CreateProjectStructure(string root, string[] folders, string templateName)
        {
            PrintHeader("5. 執行建構作業");
            Console.WriteLine($"📍 根目錄: {root}\n");
            
            Directory.CreateDirectory(root);

            foreach (string folder in folders)
            {
                string fullPath = Path.Combine(root, folder);
                Directory.CreateDirectory(fullPath);
                File.WriteAllText(Path.Combine(fullPath, ".gitkeep"), "");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("  [DIR] ");
                Console.ResetColor();
                Console.WriteLine(folder.Replace("/", " ── "));
            }

            GenerateReadme(root, templateName, folders);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n⭐ 專案初始化作業完成。");
            Console.ResetColor();
        }

        static void GenerateReadme(string root, string templateName, string[] folders)
        {
            string filePath = Path.Combine(root, "README.md");
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"# {Path.GetFileName(root)}");
            sb.AppendLine($"\n> 初始化時間: {DateTime.Now:yyyy-MM-dd HH:mm}");
            sb.AppendLine($"\n## 📋 套用模板: {templateName}");
            sb.AppendLine("\n### 📂 目錄結構\n```text");
            foreach (var folder in folders) sb.AppendLine(folder);
            sb.AppendLine("```\n\n---\n*Generated by QuickPrep*");

            File.WriteAllText(filePath, sb.ToString());
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("  [FILE] README.md (已產生)");
            Console.ResetColor();
        }

        static void PrintSystemMsg(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"[系統] {msg}");
            Console.ResetColor();
        }
    }
}

public class ProjectTemplate
{
    public string Name { get; set; } = "";
    public string[] Folders { get; set; } = Array.Empty<string>();
}