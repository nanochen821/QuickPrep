using System;
using System.IO;
using System.Linq;

namespace QuickPrep
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== QuickPrep 專案初始化工具 v1.1 ===");
            
            Console.Write("請輸入專案名稱: ");
            // 修正 1: 加入 ? 並處理 null 情況
            string? projectName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(projectName))
            {
                Console.WriteLine("錯誤: 名稱不可為空。");
                return;
            }

            Console.WriteLine("\n請選擇模板類型:");
            Console.WriteLine("[1] Web 開發 (src, assets/css, assets/js)");
            Console.WriteLine("[2] Python 數據分析 (data/raw, notebooks, src)");
            Console.WriteLine("[3] 手動自定義 (支援多層級，如: src/utils)");
            Console.Write("請輸入編號 (1-3): ");
            
            // 修正 2: 統一處理可能的 null
            string choice = Console.ReadLine() ?? "";
            string[] foldersToCreate = Array.Empty<string>();

            switch (choice)
            {
                case "1":
                    foldersToCreate = new[] { "src", "assets/css", "assets/js", "docs" };
                    break;
                case "2":
                    foldersToCreate = new[] { "data/raw", "data/processed", "notebooks", "src" };
                    break;
                case "3":
                    Console.WriteLine("\n--- 進入手動模式 ---");
                    Console.WriteLine("請輸入完整路徑（例如：app/src/utils）");
                    Console.WriteLine("規則：");
                    Console.WriteLine("1. 輸入 'done' 結束。");
                    Console.WriteLine("2. 一次輸入一條完整路徑。");
                    Console.WriteLine("3. 程式會自動幫你補齊中間缺少的資料夾。");
                    
                    var customPaths = new List<string>();
                    while (true)
                    {
                        Console.Write("輸入路徑 > ");
                        string? input = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "done") break;
                        
                        customPaths.Add(input.Trim());
                    }
                    foldersToCreate = customPaths.ToArray();
                    break;
                default:
                    Console.WriteLine("無效選擇，程式結束。");
                    return;
            }

            CreateProjectStructure(projectName, foldersToCreate);
        }

        static void CreateProjectStructure(string root, string[] folders)
        {
            Console.WriteLine($"\n[開始建立專案: {root}]");

            // 1. 先建立根目錄
            Directory.CreateDirectory(root);

            // 2. 排序路徑（讓顯示起來更像樹狀）
            var sortedFolders = folders.OrderBy(f => f).ToArray();

            foreach (string folder in sortedFolders)
            {
                string fullPath = Path.Combine(root, folder);
                Directory.CreateDirectory(fullPath);
                
                // 讓顯示更漂亮：把 '/' 換成 ' └─ '
                string prettyPath = folder.Replace("/", " └─ ").Replace("\\", " └─ ");
                Console.WriteLine($"  {prettyPath}");
            }
            
            Console.WriteLine("\n✅ 專案結構已成功建立！");
        }
    }
}