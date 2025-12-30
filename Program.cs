using System;
using System.IO;

namespace QuickPrep
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== QuickPrep 專案初始化工具 v1.0 ===");
            
            // 1. 讓使用者輸入專案名稱
            Console.Write("請輸入專案名稱: ");
            string projectName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(projectName))
            {
                Console.WriteLine("錯誤: 名稱不可為空。");
                return;
            }

            // 2. 顯示模板選單
            Console.WriteLine("\n請選擇模板類型:");
            Console.WriteLine("[1] Web 開發 (src, css, js, index.html)");
            Console.WriteLine("[2] Python 數據分析 (data, notebooks, src)");
            Console.WriteLine("[3] 手動自定義 (輸入資料夾路徑，以逗號分隔)");
            Console.Write("請輸入編號 (1-3): ");
            
            string choice = Console.ReadLine();
            string[] foldersToCreate = { };

            // 3. 根據編號設定要建立的資料夾
            switch (choice)
            {
                case "1":
                    foldersToCreate = new[] { "src", "assets/css", "assets/js", "docs" };
                    break;
                case "2":
                    foldersToCreate = new[] { "data/raw", "data/processed", "notebooks", "src" };
                    break;
                case "3":
                    Console.Write("請輸入要建立的路徑 (例如: src, test, docs/pdf): ");
                    string customInput = Console.ReadLine();
                    foldersToCreate = customInput.Split(','); // 以逗號切開
                    break;
                default:
                    Console.WriteLine("無效選擇，程式結束。");
                    return;
            }

            // 4. 執行建立邏輯
            CreateProjectStructure(projectName, foldersToCreate);
        }

        static void CreateProjectStructure(string root, string[] folders)
        {
            try
            {
                foreach (string folder in folders)
                {
                    // 去除前後空白並合併路徑
                    string path = Path.Combine(root, folder.Trim());
                    Directory.CreateDirectory(path);
                    Console.WriteLine($"[建立成功] {path}");
                }
                Console.WriteLine("\n所有任務已完成！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[錯誤] 建立失敗: {ex.Message}");
            }
        }
    }
}