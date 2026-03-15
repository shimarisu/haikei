using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Haikei
{
    public partial class HaikeiForm : Form
    {
        private bool _showHelp = false;

        public HaikeiForm()
        {
            InitializeComponent();

            // 1. 全画面表示設定 (タスクバーを隠さないようにWorkingAreaに合わせる)
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Normal;
            this.StartPosition = FormStartPosition.Manual;
            this.Bounds = Screen.PrimaryScreen.WorkingArea;
            
            // 2. 背景色を初期化 (System Themeを反映)
            if (IsWindowsInDarkMode())
            {
                this.BackColor = Color.FromArgb(0x30, 0x30, 0x30);
            }
            else
            {
                this.BackColor = Color.White;
            }

            // 3. キーイベントをフォームが受け取るように設定
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form_KeyDown);

            // 4. クリックイベントと描画設定
            this.Click += new EventHandler(Form_Click);
            this.Paint += new PaintEventHandler(Form_Paint);
            this.DoubleBuffered = true;

            // 5. アプリケーションアイコンを設定
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch { /* アイコン取得失敗時は何もしない */ }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            // タスクバーを隠さないようにWorkingAreaにサイズを合わせる
            Rectangle area = Screen.GetWorkingArea(this);
            this.SetBounds(area.X, area.Y, area.Width, area.Height);
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            // ESCキーまたはQキーが押されたら終了
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Q)
            {
                this.Close();
            }
            // Hキーでヘルプの表示切り替え
            else if (e.KeyCode == Keys.H)
            {
                _showHelp = !_showHelp;
                this.Invalidate();
            }
            // スペースキーで背景色切り替え
            else if (e.KeyCode == Keys.Space)
            {
                if (this.BackColor == Color.White)
                {
                    // ダークモード (#303030)
                    this.BackColor = Color.FromArgb(0x30, 0x30, 0x30);
                }
                else
                {
                    // 白に戻す
                    this.BackColor = Color.White;
                }
                // 背景色変更時に再描画を強制
                this.Invalidate();
            }
        }

        private void Form_Click(object sender, EventArgs e)
        {
            _showHelp = !_showHelp;
            this.Invalidate();
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            if (_showHelp)
            {
                // テキストの描画品質を向上（クリアタイプ）
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // 背景色の輝度から文字色を決定
                bool isBright = (this.BackColor.R * 0.299 + this.BackColor.G * 0.587 + this.BackColor.B * 0.114) > 128;
                Color textColor = isBright ? Color.Black : Color.White;
                Color keyBackColor = isBright ? Color.WhiteSmoke : Color.FromArgb(60, 60, 60);
                Color keyBorderColor = isBright ? Color.LightGray : Color.Gray;

                // フォントをNoto Sansに変更
                FontFamily fontFamily;
                try { fontFamily = new FontFamily("Noto Sans CJK JP"); }
                catch { try { fontFamily = new FontFamily("Noto Sans JP"); }
                catch { fontFamily = new FontFamily("Meiryo UI"); } }

                using (Font font = new Font(fontFamily, 18, FontStyle.Regular))
                using (Brush textBrush = new SolidBrush(textColor))
                using (Brush keyBackBrush = new SolidBrush(keyBackColor))
                using (Pen keyBorderPen = new Pen(keyBorderColor, 1.5f))
                {
                    // 描画内容（テキストとキーの分割）
                    var lines = new[] {
                        new object[] { "白黒反転は ", "Space", " キー、終了は ", "ESC", " キーを押してください。" },
                        new object[] { "Press ", "Space", " to toggle color. Press ", "ESC", " to exit." }
                    };

                    float lineHeight = font.GetHeight(e.Graphics) * 1.6f;
                    float totalHeight = lines.Length * lineHeight;
                    float startY = (this.ClientSize.Height - totalHeight) / 2;

                    for (int i = 0; i < lines.Length; i++)
                    {
                        var lineParts = lines[i];
                        float lineWidth = 0;
                        var partsSizes = new SizeF[lineParts.Length];

                        // 1. 各パーツの幅を計測
                        for (int j = 0; j < lineParts.Length; j++)
                        {
                            string text = (string)lineParts[j];
                            SizeF size = e.Graphics.MeasureString(text, font, 1000, StringFormat.GenericTypographic);
                            
                            // キーの場合は余白を追加
                            if (text == "Space" || text == "ESC")
                            {
                                size.Width += 24; // 左右余白
                            }
                            partsSizes[j] = size;
                            lineWidth += size.Width;
                        }

                        // 2. 描画開始X座標（中央揃え）
                        float currentX = (this.ClientSize.Width - lineWidth) / 2;
                        float currentY = startY + (i * lineHeight);

                        // 3. 描画実行
                        for (int j = 0; j < lineParts.Length; j++)
                        {
                            string text = (string)lineParts[j];
                            SizeF size = partsSizes[j];
                            bool isKey = (text == "Space" || text == "ESC");

                            if (isKey)
                            {
                                // キーの背景（角丸）を描画
                                float keyHeight = font.Height + 2;
                                float keyY = currentY + (lineHeight - keyHeight) / 2 - 2;
                                
                                RectangleF keyRect = new RectangleF(currentX + 4, keyY, size.Width - 8, keyHeight);
                                DrawRoundedRectangle(e.Graphics, keyBorderPen, keyBackBrush, keyRect, 6);

                                // 文字を中央に
                                float textX = currentX + (size.Width - e.Graphics.MeasureString(text, font, 1000, StringFormat.GenericTypographic).Width) / 2;
                                // 文字の垂直位置調整
                                float textY = keyY + (keyHeight - font.Height) / 2 + 1;
                                e.Graphics.DrawString(text, font, textBrush, textX, textY, StringFormat.GenericTypographic);
                            }
                            else
                            {
                                // 通常テキスト
                                float textY = currentY + (lineHeight - font.Height) / 2;
                                e.Graphics.DrawString(text, font, textBrush, currentX, textY, StringFormat.GenericTypographic);
                            }
                            currentX += size.Width;
                        }
                    }
                }
            }
        }

        private void DrawRoundedRectangle(Graphics g, Pen pen, Brush brush, RectangleF rect, float radius)
        {
            float diameter = radius * 2;
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();

                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }
        }

        private bool IsWindowsInDarkMode()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        var val = key.GetValue("AppsUseLightTheme");
                        if (val != null)
                        {
                            return (int)val == 0;
                        }
                    }
                }
            }
            catch { }
            return false;
        }
    }
}

