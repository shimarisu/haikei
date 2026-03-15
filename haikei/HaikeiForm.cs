using System;
using System.Drawing;
using System.Windows.Forms;

namespace Haikei
{
    public partial class HaikeiForm : Form
    {
        private bool _showHelp = false;

        public HaikeiForm()
        {
            InitializeComponent();

            // 1. 全画面表示設定
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            
            // 2. 背景色を白に設定
            this.BackColor = Color.White;

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
            // ESCキーが押されたら終了
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
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
                string message = "白黒反転はスペースキー、終了はESCキーを押してください。\n" +
                                 "Press Space to toggle color. Press ESC to exit.";
                
                // 背景色の輝度から文字色を決定
                Color textColor = (this.BackColor.R * 0.299 + this.BackColor.G * 0.587 + this.BackColor.B * 0.114) > 128 
                                  ? Color.Black 
                                  : Color.White;

                using (Font font = new Font("Yu Gothic UI", 18, FontStyle.Regular))
                using (Brush brush = new SolidBrush(textColor))
                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    e.Graphics.DrawString(message, font, brush, this.ClientRectangle, sf);
                }
            }
        }
    }
}

