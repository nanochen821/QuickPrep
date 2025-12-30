🚀 QuickPrep v2.8
一個為專業開發者打造的 C# 專案初始化 CLI 工具
QuickPrep 是一個輕量化、高效且具備高度擴充性的命令列工具，旨在幫助開發者在秒級時間內建立規範化的專案結構。透過自定義 JSON 模板與智能路徑解析，它能消除重複的勞動，讓你專注於編寫核心代碼。

✨ 核心亮點
📦 雙層模板系統：同時支援讀取程式目錄與當前工作目錄的 templates.json，實現全域配置與專案自定義的完美結合。

⌨️ 智能路徑解析：

空格拆分：一次輸入多個路徑（如 src docs tests）。

大括號擴展：支援 Linux 風格語法（如 assets/{css,js,img}），快速生成深層結構。

🔍 建構前預覽：在正式寫入磁碟前提供完整的結構預覽清單，確保操作精準無誤。

📄 自動生成文件：自動建立 .gitkeep 確保 Git 追蹤空目錄，並產出內容詳盡的 README.md。

🎨 專業終端 UI：具備 ANSI 顏色導引、ASCII Art 標題以及 UTF-8 編碼支援，提供極致的視覺與互動體驗。

🛠️ 安裝與執行
直接運行 (Standalone)
你可以將編譯後的 QuickPrep.exe 放在任何地方，建議將其路徑加入系統的 Path 環境變數中，即可在任何終端機直接輸入 QuickPrep 使用。

開發環境編譯
Bash

# 複製專案
git clone https://github.com/nanochen821/QuickPrep.git

# 進入資料夾
cd QuickPrep

# 運行專案
dotnet run
⚙️ 模板配置說明 (templates.json)
你可以透過修改 templates.json 來擴充你常用的開發架構：

JSON

[
  {
    "Name": "Backend API",
    "Folders": ["src/Controllers", "src/Models", "src/Services", "tests/UnitTests"]
  },
  {
    "Name": "Python Data Science",
    "Folders": ["data/raw", "notebooks", "src", "reports/figures"]
  }
]

📅 版本演進
v2.8 - 引入雙路徑加載邏輯、預覽確認機制與智能路徑擴展。

v2.5 - UI 視覺大更新，引入 ConsoleColor 視覺引導。

v2.0 - 實作外部 JSON 模板讀取與 README 自動生成功能。

v1.0 - 初始版本，基礎路徑建立功能。

📄 開源協議
本專案採用 MIT License 協議。