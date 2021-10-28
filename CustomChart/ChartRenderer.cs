using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;

namespace CustomChart
{
    public class ChartRenderer
    {
        private int width = 0, height = 0;
        private Bitmap bmp = null;
        private Graphics gfx = null;
        private List<ChartData> data = null;
        private int iconWidth = 10;
        private int iconHeight = 10;
        private double totHeight = 300;
        List<ChartData> alerts = null;
        List<ChartData> successes = null;
        List<ChartData> errors = null;
        List<ChartData> failures = null;
        List<ChartData> undetermineds = null;


        Image successImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "success.png"));
        Image alertImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "alert.png"));
        Image errorImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "error.png"));
        Image failureImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "failure.png"));
        Image undeterminedImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "undetermined.png"));
        Image logoImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "logo.png"));
        Image yelloFingerImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "yellow_finger.png"));
        Image redFingerImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "red_finger.png"));
        Image Img14 = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "14.png"));
        public ChartRenderer(int width, int height)
        {
            this.width = width;
            this.height = height;

            List<ChartData> alerts = new List<ChartData>();
            List<ChartData> successes = new List<ChartData>();
            List<ChartData> errors = new List<ChartData>();
            List<ChartData> failures = new List<ChartData>();
            List<ChartData> undetermineds = new List<ChartData>();
        }

        public void setChatData( List<ChartData> data)
        {
            this.data = data;
            undetermineds = data.Where(e => e.percentage < 0).ToList();
            alerts = data.Where(e => e.percentage >= 0 && e.percentage < 300).ToList();
            successes = data.Where(e => e.percentage >= 300 && e.percentage < 700).ToList();
            errors = data.Where(e => e.percentage >= 700 && e.percentage < 1000).ToList();
            failures = data.Where(e => e.percentage >= 1000).ToList();

        }

        public Bitmap getBmp()
        {
            return this.bmp;
        }
        public void drawCenteredString(string content, Rectangle rect, Brush brush, Font font)
        {

            //using (Font font1 = new Font("Calibri", fontSize, FontStyle.Bold, GraphicsUnit.Point))

            // Create a StringFormat object with the each line of text, and the block
            // of text centered on the page.
            double px = height / totHeight;
            rect.Location = convertCoord(rect.Location);
            rect.Width = (int)(px * rect.Width);
            rect.Height = (int)(px * rect.Height);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // Draw the text and the surrounding rectangle.
            gfx.DrawString(content, font, brush, rect, stringFormat);
            //gfx.DrawRectangle(Pens.Black, rect);

        }
        public void drawCenteredString_withBorder(string content, Rectangle rect, Brush brush, Font font, Color borderColor)
        {

            //using (Font font1 = new Font("Calibri", fontSize, FontStyle.Bold, GraphicsUnit.Point))

            // Create a StringFormat object with the each line of text, and the block
            // of text centered on the page.
            double px = height / totHeight;
            rect.Location = convertCoord(rect.Location);
            rect.Width = (int)(px * rect.Width);
            rect.Height = (int)(px * rect.Height);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // Draw the text and the surrounding rectangle.
            gfx.DrawString(content, font, brush, rect, stringFormat);

            Pen borderPen = new Pen(new SolidBrush(borderColor), 2);
            gfx.DrawRectangle(borderPen, rect);
            borderPen.Dispose();
        }
        public void draw()
        {

            drawHex();

            if (bmp == null)
                bmp = new Bitmap(width, height);

            if (gfx == null)
                gfx = Graphics.FromImage(bmp);



            drawImg(logoImg, new Point(-125, -110), new Size(100, 100));
            if (data == null) return;


            int per = (int)((undetermineds.Count / (double)data.Count) * 100);
            float pxRating = 1.5f;
            drawBar(undeterminedImg, new Point(115, -100 + (int)(pxRating * per)), per, Color.Black);

            per = (int)((successes.Count / (double)data.Count) * 100);
            drawBar(successImg, new Point(145, -100 + (int)(pxRating * per)), per, Color.Green);

            per = (int)((alerts.Count / (double)data.Count) * 100);
            drawBar(alertImg, new Point(175, -100 + (int)(pxRating * per)), per, Color.Orange);


            per = (int)((errors.Count / (double)data.Count) * 100);
            if (per != 0) drawBar(errorImg, new Point(205, -100 + (int)(pxRating * per)), per, Color.Red, true);
            else drawBar(errorImg, new Point(205, -100 + (int)(pxRating * per)), per, Color.Red);


            per = (int)((failures.Count / (double)data.Count) * 100);
            if (per !=0) drawBar(failureImg, new Point(235, -100 + (int)(pxRating * per)), per, Color.DarkRed, true);
            else drawBar(failureImg, new Point(235, -100 + (int)(pxRating * per)), per, Color.DarkRed);


            //Draw 14 Image and success count
            Font h1font = new Font("Arial", 30, FontStyle.Bold);
            drawCenteredString(successes.Count.ToString(), new Rectangle(280, -10, 70, 40), Brushes.Black, h1font);
            //drawString(new Point(280, -20), successes.Count.ToString(), h1font, Brushes.Black);
            if (errors.Count > 0 && failures.Count > 0)
            {
                drawImg(Img14, new Point(290, -50), new Size(50, 50));
                drawCenteredString_withBorder((errors.Count + failures.Count).ToString(), new Rectangle(280, -100, 70, 40), Brushes.Black, h1font, Color.Red);
            }
                


            drawChart(successImg, successes, 0);
            drawChart(alertImg, alerts, 25);
            drawChart(errorImg, errors, 50);
            drawChart(failureImg, failures, 75);
            //drawImg(successImg, new Point(29 - iconWidth /2, iconHeight / 2));
            ////gfx.DrawImage(successImg, convertCoord(new Point(29, 0)));
            //int x = 0;

            drawFirstPie();
            drawSecondPie();
            drawLeadIcons();

            h1font.Dispose();
        }

        public void drawFirstPie() {

            drawString(new Point(365, 50), "Highest OaC");
            Image img = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "highest_ooc.png"));
            drawImg(img, new Point(350, 30), new Size(100, 50));


            int maxPer = 0;
            if (failures != null && failures.Count > 0) maxPer = Math.Max(maxPer, failures.Max(e => e.percentage));
            drawString(new Point(375, 0), string.Format("{0}%", maxPer), 20);

            if (maxPer >= 2000)
            {
                drawImg(redFingerImg, new Point(320, 50), new Size(50, 50));
            }
        }

        public void drawSecondPie()
        {

            drawString(new Point(365, -30), "Average OaC");
            Image img = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "average_ooc.png"));
            drawImg(img, new Point(350, -50), new Size(100, 50));

            double sum = 0, cnt = 0;
            if (failures != null) {
                sum = sum + failures.Sum(e => e.percentage);
                cnt = cnt + failures.Count;
            }
            if (errors != null)
            {
                sum = sum + errors.Sum(e => e.percentage);
                cnt = cnt + errors.Count;
            }
            if (cnt == 0)
            {
                sum = 0; cnt = 1;
            }
            drawString(new Point(375, -100), string.Format("{0}%", (int)(sum/cnt)), 20);


        }
        public void drawLeadIcons()
        {

            drawImg(undeterminedImg, new Point(0 , -120 ), new Size(15, 15));
            drawString(new Point(-15, -140), "Undetermined", 8);

            drawImg(successImg, new Point(55, -120), new Size(15, 15));
            drawString(new Point(50, -140), "Calibrated", 8);

            drawImg(alertImg, new Point(110, -120), new Size(15, 15));
            drawString(new Point(100, -140), "Not Calibrated", 8);

            drawImg(errorImg, new Point(165, -120), new Size(15, 15));
            drawString(new Point(155, -140), "Disqualified", 8);

            drawImg(failureImg, new Point(230, -120), new Size(15, 15));
            drawString(new Point(220, -140), "Unreadable", 8);
        }
        public void drawIconsRegion(Image img, int al1, int al2, int minRadius, int num)
        {
            int per = (int)Math.Round(num * 100 / (double) data.Count);
            Random rand = new Random();

            for (int i = 0; i< per; i++)
            {
                int alpha = rand.Next(al1, al2);
                int beta = alpha % 60;
                int minR, maxR;

                if (beta > 30)
                {
                    beta = 60 - beta;
                }

                minR = (int)(minRadius / Math.Cos( Helper.DegreesToRadians ( beta) ));
                maxR = (int)((minRadius + 25) / Math.Cos(Helper.DegreesToRadians (beta)));

                int radius = rand.Next(minR, maxR);

                int x = (int)(radius * Math.Sin(Helper.DegreesToRadians ( alpha ) ));
                int y = (int)(radius * Math.Cos(Helper.DegreesToRadians(alpha) ));

                drawImg(img, new Point(x - iconWidth / 2, y + iconWidth / 2), new Size(iconWidth, iconHeight));
            }

        }
        public void drawChart(Image img, List<ChartData> drawdata, int minRadius)
        {
            for (int i = 0; i < 16; i++)
            {
                int alpha1 = (int)(-11.25 + 22.5 * i);
                int alpha2 = (int)( 11.25 + 22.5 * i);

                List<ChartData> items;
                switch (i)
                {
                    case 0:
                        items = drawdata.Where(e => e.deviation_direction == "N").ToList();
                        drawIconsRegion(img, alpha1, alpha2, minRadius, items.Count);
                        break;
                    case 1:
                        items = drawdata.Where(e => e.deviation_direction == "NNE").ToList();
                        drawIconsRegion(img, alpha1, alpha2, minRadius, items.Count);
                        break;
                    case 2:
                        items = drawdata.Where(e => e.deviation_direction == "NE").ToList();
                        drawIconsRegion(img, alpha1, alpha2, minRadius, items.Count);
                        break;
                    case 3:
                        items = drawdata.Where(e => e.deviation_direction == "ENE").ToList();
                        drawIconsRegion(img, alpha1, alpha2, minRadius, items.Count);
                        break;
                    case 4:
                        items = drawdata.Where(e => e.deviation_direction == "E").ToList();
                        drawIconsRegion(img, alpha1, alpha2, minRadius, items.Count);
                        break;
                    case 5:
                        items = drawdata.Where(e => e.deviation_direction == "ESE").ToList();
                        drawIconsRegion(img, alpha1, alpha2, minRadius, items.Count);
                        break;
                    case 6:
                        items = drawdata.Where(e => e.deviation_direction == "SE").ToList();
                        drawIconsRegion(img, alpha1, alpha2, minRadius, items.Count);
                        break;
                    case 7:
                        items = drawdata.Where(e => e.deviation_direction == "SSE").ToList();
                        drawIconsRegion(img, alpha1, alpha2, minRadius, items.Count);
                        break;
                    case 8:
                        items = drawdata.Where(e => e.deviation_direction == "S").ToList();
                        drawIconsRegion(img, alpha1, alpha2, minRadius, items.Count);
                        break;
                    case 9:
                        items = drawdata.Where(e => e.deviation_direction == "SSW").ToList();
                        drawIconsRegion(img, alpha1, alpha2, minRadius, items.Count);
                        break;
                    case 10:
                        items = drawdata.Where(e => e.deviation_direction == "SW").ToList();
                        drawIconsRegion(img, alpha1, alpha2, minRadius, items.Count);
                        break;
                    case 11:
                        items = drawdata.Where(e => e.deviation_direction == "WSW").ToList();
                        drawIconsRegion(img, alpha1, alpha2, minRadius, items.Count);
                        break;
                    case 12:
                        items = drawdata.Where(e => e.deviation_direction == "W").ToList();
                        drawIconsRegion(img, alpha1, alpha2, minRadius, items.Count);
                        break;
                    case 13:
                        items = drawdata.Where(e => e.deviation_direction == "WNW").ToList();
                        drawIconsRegion(img, alpha1, alpha2, minRadius, items.Count);
                        break;
                    case 14:
                        items = drawdata.Where(e => e.deviation_direction == "NW").ToList();
                        drawIconsRegion(img, alpha1, alpha2, minRadius, items.Count);
                        break;
                    case 15:
                        items = drawdata.Where(e => e.deviation_direction == "NNW").ToList();
                        drawIconsRegion(img, alpha1, alpha2, minRadius, items.Count);
                        break;
                }
            }
        }
        public void drawBar(Image img, Point lt, float per, Color brushColor, bool yello_finger = false)
        {
            double px = height / totHeight;
            float pxRating = 1.5f;
            using (var pen = new System.Drawing.Pen(System.Drawing.Color.Black, 4))
            {

                Rectangle rect = new Rectangle(convertCoord(lt), new Size((int)(15 * px), (int)(per * pxRating * px)));
                //gfx.DrawRectangle(pen, rect);

                if (brushColor == null)
                {
                    brushColor = Color.DarkGreen;
                }
                Brush brush = new SolidBrush(brushColor);
                gfx.FillRectangle(brush, rect);


                lt.Y = lt.Y + 20;
                drawImg(img, lt, new Size(15, 15));

                if (yello_finger)
                {
                    lt.Y += 50;
                    lt.X -= 15;
                    drawImg(yelloFingerImg, lt, new Size(45, 45));
                    lt.X += 15;
                }

                lt.Y = -100;
                drawString(lt, string.Format("{0}%", per), 10);


            }


        }
        public void drawString(Point o, string content, int font = 15)
        {
            double px = height / totHeight;
            o = convertCoord(o);

            // Create font and brush.
            Font drawFont = new Font("Arial", font);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            gfx.DrawString(content, drawFont, drawBrush, o.X, o.Y);
            drawFont.Dispose();
            drawBrush.Dispose();

        }
        public void drawString(Point o, string content, Font font, Brush brush)
        {
            double px = height / totHeight;
            o = convertCoord(o);
            gfx.DrawString(content, font, brush, o.X, o.Y);
        }
        public void drawImg( Image img, Point o, Size size)
        {
            double px = height / totHeight;
            o = convertCoord(o);
            Rectangle rect = new Rectangle(o, new Size((int)(size.Width * px), (int)(size.Height * px)));
            gfx.DrawImage(img, rect);
        }
        public Point convertCoord ( Point a)
        {
            double px = height / totHeight;

            Point res = new Point();
            res.X = (int) ((a.X + 125) * px);
            res.Y = (int) (( 125 - a.Y) * px);
            return res;
        }
        public void drawHex()
        {
            if (bmp == null)
                bmp = new Bitmap(width, height);

            if (gfx == null)
                gfx = Graphics.FromImage(bmp);

            using (var dash_pen = new System.Drawing.Pen(System.Drawing.Color.Black))
            {
                // draw one thousand random white lines on a dark blue background
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gfx.Clear(System.Drawing.Color.White);


                dash_pen.DashStyle = DashStyle.Dash;
                var pt1 = convertCoord ( new Point(-145, 0) );
                var pt2 = convertCoord ( new Point( 145, 0) );
                gfx.DrawLine(dash_pen, pt1, pt2);

                pt1 = convertCoord(new Point(-72, -125));
                pt2 = convertCoord(new Point(72, 125));
                gfx.DrawLine(dash_pen, pt1, pt2);


                pt1 = convertCoord(new Point(-72, 125));
                pt2 = convertCoord(new Point(72, -125));
                gfx.DrawLine(dash_pen, pt1, pt2);



                //Draw Center Cross
                var pen = new Pen(Color.Black);
                pen.Width = 8;

                pt1 = convertCoord(new Point(-10, 0));
                pt2 = convertCoord(new Point(10, 0));
                gfx.DrawLine(pen, pt1, pt2);

                pt1 = convertCoord(new Point(0, 10));
                pt2 = convertCoord(new Point(0, -10));
                gfx.DrawLine(pen, pt1, pt2);


                //Draw Bold Hex width 8
                Point[] hex = new Point[6];
                pen.Width = 4;
                hex[0] = convertCoord(new Point(-58, 100));
                hex[1] = convertCoord(new Point(58, 100));
                hex[2] = convertCoord(new Point(115, 0));
                hex[3] = convertCoord(new Point(58, -100));
                hex[4] = convertCoord(new Point(-58, -100));
                hex[5] = convertCoord(new Point(-115, 0));

                gfx.DrawPolygon(pen, hex);


                //Draw Bold Hex width 6
                pen.Width = 4;

                hex[0] = convertCoord(new Point(-43, 75));
                hex[1] = convertCoord(new Point(43, 75));
                hex[2] = convertCoord(new Point(86, 0));
                hex[3] = convertCoord(new Point(43, -75));
                hex[4] = convertCoord(new Point(-43, -75));
                hex[5] = convertCoord(new Point(-86, 0));

                gfx.DrawPolygon(pen, hex);


                //Draw Bold Hex width 4
                pen.Width = 4;

                hex[0] = convertCoord(new Point(-29, 50));
                hex[1] = convertCoord(new Point(29, 50));
                hex[2] = convertCoord(new Point(58, 0));
                hex[3] = convertCoord(new Point(29, -50));
                hex[4] = convertCoord(new Point(-29, -50));
                hex[5] = convertCoord(new Point(-58, 0));

                gfx.DrawPolygon(pen, hex);


                //Draw Bold Hex width 2
                pen.Width = 8;
                pen.Color = Color.Green;
                hex[0] = convertCoord(new Point(-14, 25));
                hex[1] = convertCoord(new Point(14, 25));
                hex[2] = convertCoord(new Point(29, 0));
                hex[3] = convertCoord(new Point(14, -25));
                hex[4] = convertCoord(new Point(-14, -25));
                hex[5] = convertCoord(new Point(-29, 0));

                gfx.DrawPolygon(pen, hex);

                pen.Dispose();
            }
        }


    }
}
