using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Point pMouseDown = new Point(0, 0);
        PointF pCanvasOffset = new PointF(0.0f, 0.0f);
        Point pMouseMoveDelta = new Point(0, 0);
        RectangleF rCanvas;
        bool bMouseDown = false;
        float fZoom = 1.0f;
        Panel oCanvas;
        bool bFoundPanel = false;
        string[] sWeekdays = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        string[] sMonths = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            oCanvas = Controls.Find("panel1", true).FirstOrDefault() as Panel;
            oCanvas.MouseWheel += new MouseEventHandler(panel1_MouseWheel);
            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, oCanvas, new object[] { true });
            rCanvas = new RectangleF(new Point(0, 0), oCanvas.Size);
            bFoundPanel = true;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawString("Zoom: " + fZoom.ToString("N5"), this.Font, Brushes.Black, 0, 0);

            float fCanvasOffsetX = pCanvasOffset.X + pMouseMoveDelta.X;
            float fCanvasOffsetY = pCanvasOffset.Y + pMouseMoveDelta.Y;

            float nCellWidth = 25.0f * fZoom;
            float nCellHeight = 17.5f * fZoom;
            float nYearTitleHeight = nCellHeight * 3;
            float nMonthTitleHeight = nCellHeight;
            float nMonthWeekHeight = nCellHeight / 3;
            float nMonthDayOffset = nMonthTitleHeight + nMonthWeekHeight;
            float nMonthPaddingHorizontal = nCellWidth;
            float nMonthPaddingVertical = nCellHeight;
            float nMonthSpacingHorizontal = nCellWidth;
            float nMonthSpacingVertical = nCellHeight;
            float nYearSpacingHorizontal = nCellWidth * 2;
            float nYearSpacingVertical = nCellHeight * 3;
            bool bHourVertical = true;

            float nTableWidth = 7.0f * nCellWidth;
            float nTableHeight = 6.0f * nCellHeight;
            float fCanvasWidth = nCellWidth * 7.0f;
            float fCanvasHeight = nCellHeight * 6.0f + nMonthDayOffset;
            float nMonthHorizontal = 4.0f;
            float nMonthVertical = 3.0f;
            float nYearHorizontal = 3.0f;
            float nYearVertical = 3.0f;
            float nHourHeight = nCellHeight / 24;
            float nHourWidth = nCellWidth / 24;
            float nDayHeight = nCellHeight / 6;
            float nWeekHeight = nCellHeight / 4;
            Pen penBlack = new Pen(Color.FromArgb(255, 0, 0, 0));
            Pen penLightGray = new Pen(Color.LightGray);
            Font fHour = new Font(this.Font.Name, nHourHeight * 0.66f, this.Font.Style, this.Font.Unit);
            Font fDay = new Font(this.Font.Name, nDayHeight * 0.66f, this.Font.Style, this.Font.Unit);
            Font fWeek = new Font(this.Font.Name, nWeekHeight * 0.66f, this.Font.Style, this.Font.Unit);
            Font fMonth = new Font(this.Font.Name, nMonthTitleHeight * 0.66f, this.Font.Style, this.Font.Unit);
            Font fYear = new Font(this.Font.Name, nYearTitleHeight * 0.66f, this.Font.Style, this.Font.Unit);
            PointF pHourPosition;
            PointF pDayPosition;
            PointF pWeekPosition;

            float nYearWidth = nMonthPaddingHorizontal * 2 + fCanvasWidth * nMonthHorizontal + nMonthSpacingHorizontal * (nMonthHorizontal - 1);
            float nYearHeight = nMonthPaddingVertical * 2 + nYearTitleHeight + fCanvasHeight * nMonthVertical + nMonthSpacingVertical * (nMonthVertical - 1);
            
            //
            for (int m = 0; m < nYearHorizontal; m++)
            for (int n = 0; n < nYearVertical; n++)
            {
                float nYearOffsetX = fCanvasOffsetX + (nYearWidth + nYearSpacingHorizontal) * m;
                float nYearOffsetY = fCanvasOffsetY + (nYearHeight + nYearSpacingVertical) * n;
                
                if (rCanvas.IntersectsWith(new RectangleF(nYearOffsetX, nYearOffsetY, nYearWidth, nYearHeight))){
                    float nYearEndX = nYearOffsetX + nYearWidth;
                    float nYearEndY = nYearOffsetY + nYearHeight;
                    e.Graphics.DrawLine(penBlack, nYearOffsetX, nYearOffsetY, nYearEndX, nYearOffsetY);
                    e.Graphics.DrawLine(penBlack, nYearOffsetX, nYearOffsetY + nYearTitleHeight, nYearEndX, nYearOffsetY + nYearTitleHeight);
                    e.Graphics.DrawLine(penBlack, nYearOffsetX, nYearEndY, nYearEndX, nYearEndY);
                    e.Graphics.DrawLine(penBlack, nYearOffsetX, nYearOffsetY, nYearOffsetX, nYearEndY);
                    e.Graphics.DrawLine(penBlack, nYearEndX, nYearOffsetY, nYearEndX, nYearEndY);

                    if (nYearTitleHeight > 8.0f)
                    if (rCanvas.IntersectsWith(new RectangleF(nYearOffsetX, nYearOffsetY, nYearWidth, nYearTitleHeight)))
                    e.Graphics.DrawString((m + n * nYearHorizontal + 2020).ToString(), fYear, Brushes.Black, nYearOffsetX, nYearOffsetY);

                    for (int k = 0; k < nMonthHorizontal; k++)
                    for (int l = 0; l < nMonthVertical; l++)
                    {
                        float nTableOffsetX = nMonthPaddingHorizontal + nYearOffsetX + k * (nMonthSpacingHorizontal + fCanvasWidth);
                        float nTableOffsetY = nMonthPaddingVertical + nYearTitleHeight + nYearOffsetY + l * (nMonthSpacingVertical + fCanvasHeight);

                        if (rCanvas.IntersectsWith(new RectangleF(nTableOffsetX, nTableOffsetY, nTableWidth, nTableHeight + nMonthDayOffset)))
                        {
                            if (nHourHeight > 8.0f)
                            for (int i = 0; i < 6; i++)
                            {
                                for (int h = 0; h < 7; h++) {
                                    pHourPosition = new PointF(nTableOffsetX + nCellWidth * h, nCellHeight * i + nTableOffsetY + nMonthDayOffset);
                                    if (rCanvas.IntersectsWith(new RectangleF(pHourPosition.X, pHourPosition.Y, nCellWidth, nHourHeight)))
                                    e.Graphics.DrawString(bHourVertical ? "0" : "12 AM", fHour, Brushes.Gray, pHourPosition);
                                }
                                for (int j = 1; j < 24; j++)
                                {
                                    if (bHourVertical)
                                    {
                                        string sHour = j.ToString();
                                        for (int h = 0; h < 7; h++) {
                                            float nHourOffsetX = nTableOffsetX + nCellWidth * h + nHourWidth * j;
                                            e.Graphics.DrawLine(penLightGray,
                                                nHourOffsetX, nTableOffsetY + nMonthDayOffset,
                                                nHourOffsetX, nTableOffsetY + nMonthDayOffset + nTableHeight);

                                            pHourPosition = new PointF(nHourOffsetX, nTableOffsetY + nMonthDayOffset + nCellHeight * i);
                                            if (rCanvas.IntersectsWith(new RectangleF(pHourPosition.X, pHourPosition.Y, nHourWidth, nCellHeight)))
                                            e.Graphics.DrawString(sHour, fHour, Brushes.Gray, pHourPosition);
                                        }
                                    }
                                    else
                                    {
                                        string sHour = (j < 12 ? j : j - 12).ToString() + (j < 12 ? " AM" : " PM");
                                        float nHourOffsetY = nTableOffsetY + nMonthDayOffset + nCellHeight * i + nHourHeight * j;
                                        e.Graphics.DrawLine(penLightGray,
                                            nTableOffsetX, nHourOffsetY,
                                            nTableOffsetX + nTableWidth, nHourOffsetY);
                                   
                                        for (int h = 0; h < 7; h++) {
                                            pHourPosition = new PointF(nTableOffsetX + nCellWidth * h, nHourOffsetY);
                                            if (rCanvas.IntersectsWith(new RectangleF(pHourPosition.X, pHourPosition.Y, nCellWidth, nHourHeight)))
                                            e.Graphics.DrawString(sHour, fHour, Brushes.Gray, pHourPosition);
                                        }
                                    }
                                }
                            }


                            if (nDayHeight > 8.0f)
                            if (nHourHeight <= 8.0f)
                            for (int i = 0; i < 6; i++)
                            {
                                for (int h = 0; h < 7; h++)
                                {
                                    pDayPosition = new PointF(nTableOffsetX + nCellWidth * h, nCellHeight * i + nTableOffsetY + nMonthDayOffset);
                                    if (rCanvas.IntersectsWith(new RectangleF(pDayPosition.X, pDayPosition.Y, nCellWidth, nDayHeight)))
                                    e.Graphics.DrawString((h + 1 + i * 7).ToString(), fDay, Brushes.Black, pDayPosition);
                                }
                            }
                            

                            if (nWeekHeight > 8.0f)
                            for (int h = 0; h < 7; h++)
                            {
                                pWeekPosition = new PointF(nTableOffsetX + nCellWidth * h, nTableOffsetY + nMonthTitleHeight);
                                if (rCanvas.IntersectsWith(new RectangleF(pWeekPosition.X, pWeekPosition.Y, nCellWidth, nWeekHeight)))
                                e.Graphics.DrawString(sWeekdays[h], fWeek, Brushes.Black, pWeekPosition);
                            }

                            
                            if (nMonthTitleHeight > 8.0f)
                            {
                                if (rCanvas.IntersectsWith(new RectangleF(nTableOffsetX , nTableOffsetY, nCellWidth, nMonthTitleHeight)))
                                e.Graphics.DrawString(sMonths[(int)(k + l * nMonthHorizontal)], fMonth, Brushes.Black, nTableOffsetX , nTableOffsetY);
                            }

                            for (int i = 1; i <= 6; i++)
                                e.Graphics.DrawLine(penBlack,
                                    nTableOffsetX + i * nCellWidth, nTableOffsetY + nMonthTitleHeight,
                                    nTableOffsetX + i * nCellWidth, nTableOffsetY + nTableHeight + nMonthDayOffset);
                            for (int i = 0; i <= 1; i++)
                                e.Graphics.DrawLine(penBlack,
                                    nTableOffsetX + i * fCanvasWidth, nTableOffsetY,
                                    nTableOffsetX + i * fCanvasWidth, nTableOffsetY + nTableHeight + nMonthDayOffset);

                            e.Graphics.DrawLine(penBlack,
                                nTableOffsetX,               nTableOffsetY,
                                nTableOffsetX + nTableWidth, nTableOffsetY);
                            e.Graphics.DrawLine(penBlack,
                                nTableOffsetX,               nTableOffsetY + nMonthTitleHeight,
                                nTableOffsetX + nTableWidth, nTableOffsetY + nMonthTitleHeight);
                            for (int j = 0; j <= 6; j++)
                                e.Graphics.DrawLine(penBlack,
                                    nTableOffsetX,               nMonthDayOffset + nTableOffsetY + j * nCellHeight,
                                    nTableOffsetX + nTableWidth, nMonthDayOffset + nTableOffsetY + j * nCellHeight);
                        }
                    }
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            pCanvasOffset = new PointF(0.0f, 0.0f);
            fZoom = 1.0f;
            if (bFoundPanel == true) oCanvas.Invalidate();
        }

        private void panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            Point pCursor = new Point(0, 0);
            if (bMouseDown) {
                pCursor = pMouseDown;
            }
            else {
                pCursor = oCanvas.PointToClient(Cursor.Position);
            }
            
            //Calculate the zoom factor, 0.9 for zooming out, 1.11111 for zooming in.
            float fScrollDirection = ((e.Delta / 120.0f) + 1.0f) / 2.0f;
            float fZoomFactor = (9.0f  + fScrollDirection) / (10.0f - fScrollDirection);

            float fCursorPosXa = (pCanvasOffset.X - pCursor.X) / fZoom;
            float fCursorPosYa = (pCanvasOffset.Y - pCursor.Y) / fZoom;

            pCanvasOffset.X = fCursorPosXa * (fZoom * fZoomFactor) + pCursor.X;
            pCanvasOffset.Y = fCursorPosYa * (fZoom * fZoomFactor) + pCursor.Y;

            fZoom *= fZoomFactor;
            if (bFoundPanel == true) oCanvas.Invalidate();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            bMouseDown = true;
            pMouseDown = oCanvas.PointToClient(Cursor.Position);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            //Create drag limits based on canvas
            //Add middle mouse movement
            if (bMouseDown)
            {
                pMouseMoveDelta.X = oCanvas.PointToClient(Cursor.Position).X - pMouseDown.X;
                pMouseMoveDelta.Y = oCanvas.PointToClient(Cursor.Position).Y - pMouseDown.Y;
                oCanvas.Invalidate();
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            bMouseDown = false;
            pCanvasOffset.X += pMouseMoveDelta.X;
            pCanvasOffset.Y += pMouseMoveDelta.Y;
            pMouseMoveDelta.X = 0;
            pMouseMoveDelta.Y = 0;
        }

        private bool IntersectsWithLine(RectangleF tCanvas, PointF tPoint1, PointF tPoint2)
        {
            //Assumes tPoint1 <= tPoint2, works for both horizontal and vertical lines
            return tPoint2.X >= tCanvas.X && tPoint1.X <= tCanvas.X + tCanvas.Width
                && tPoint2.Y >= tCanvas.Y && tPoint1.Y <= tCanvas.Y + tCanvas.Height;
        }
    }
}

