using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using static MinishMaker.Utilities.ObjectDefinitionParser;

// Adapted from ZOLE's Grid Box by Lin22
namespace MinishMaker.UI
{
    public partial class GridBoxComponent : PictureBox
    {
        private Color hoverColor = Color.White;
        private Color selectedColor = Color.Red;
        private Size selectionSize = new Size(16, 16);
        private int selectedIndex = -1;
        public bool canSelect = true;
        public bool canHover = true;
        private bool allowMultiSelection = false;
        private Rectangle selectionRectangle = new Rectangle(0, 0, 1, 1);
        private int hoverIndex = -1;
        private int lastHoverIndex = -1;
        private int startSelection = -1;
        private Size canvas = new Size(128, 128);
        private Pen selectionPen = new Pen(Color.Red, 2);
        private InterpolationMode mode = InterpolationMode.NearestNeighbor;

        //TODO: DELETE BEFORE RELEASE
        public Point chestHighlightPoint = new Point(-1, -1);
        public Point warpHighlightPoint = new Point(-1, -1);
        public Point listObjectHighlightPoint = new Point(-1,-1);

        private Dictionary<int, Tuple<Object, Func<Object, Tuple<Point[], Brush>>>> markerDrawers = new Dictionary<int, Tuple<Object, Func<Object, Tuple<Point[], Brush>>>>();
        private int scale = 1;

        public GridBoxComponent()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        [Description("The window's update mode."), Browsable(true)]
        public InterpolationMode InterpolationMode
        {
            get { return mode; }
            set { mode = value; Invalidate(); }
        }

        [Description("The hover color."), Browsable(true)]
        public Color HoverColor
        {
            get { return hoverColor; }
            set { hoverColor = value; Invalidate(); }
        }

        [Description("The selection color."), Browsable(true)]
        public Color SelectionColor
        {
            get { return selectedColor; }
            set { selectedColor = value; selectionPen = new Pen(selectedColor, 2); this.Invalidate(); }
        }

        [Description("The box size."), Browsable(true)]
        public Size BoxSize
        {
            get { return selectionSize; }
            set { selectionSize = value; Invalidate(); }
        }

        [Description("The selected index."), Browsable(true)]
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; selectionRectangle = new Rectangle((value % (canvas.Width / selectionSize.Width)), (value / (canvas.Width / selectionSize.Width)), 1, 1); Invalidate(); }
        }

        [Description("Determines whether or not items can be selected."), Browsable(true)]
        public bool Selectable
        {
            get { return canSelect; }
            set { canSelect = value; Invalidate(); }
        }

        [Description("Determines whether or not the hoverbox will be shown."), Browsable(true)]
        public bool HoverBox
        {
            get { return canHover; }
            set { canHover = value; Invalidate(); }
        }

        [Description("The canvas size."), Browsable(true)]
        public Size CanvasSize
        {
            get { return canvas; }
            set { canvas = value; }
        }

        [Browsable(false)]
        public int HoverIndex
        {
            get { return hoverIndex; }
        }

        [Browsable(false)]
        public Rectangle SelectionRectangle
        {
            get { return selectionRectangle; }
            set { selectionRectangle = value; Invalidate(); }
        }

        [Description("Determines whether or not more than one items can be selected."), Browsable(true)]
        public bool AllowMultiSelection
        {
            get { return allowMultiSelection; }
            set { allowMultiSelection = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = mode;
            try
            {
                base.OnPaint(e);
            }
            catch (Exception) { }
        }

        private void GridBox_Paint(object sender, PaintEventArgs e)
        {
            if (canSelect)
            {
                if (!allowMultiSelection)
                {
                    if (selectedIndex != -1)
                    {
                        Point p = GetIndexPoint(selectedIndex);
                        e.Graphics.DrawRectangle(selectionPen, p.X, p.Y, selectionSize.Width, selectionSize.Height);
                    }
                }
                else
                {
                    //Point p = GetIndexPoint(selectedIndex);
                    e.Graphics.DrawRectangle(selectionPen, selectionRectangle.X * selectionSize.Width, selectionRectangle.Y * selectionSize.Height, selectionSize.Width * selectionRectangle.Width, selectionSize.Height * selectionRectangle.Height);
                }

                foreach (var pair in markerDrawers)
                {
                    var drawInformation = pair.Value;
                    var pixelFunc = drawInformation.Item2;

                    var pointData = pixelFunc(drawInformation.Item1);
                    if(pointData == null)
                    {
                        continue;
                    }
                    foreach (var point in pointData.Item1)
                    {
                        e.Graphics.FillRectangle(pointData.Item2, point.X * scale, point.Y * scale, scale, scale);
                    }
                }

                if (canHover)
                {
                    if (hoverIndex != -1)
                    {
                        Point p = GetIndexPoint(hoverIndex);
                        e.Graphics.DrawRectangle(new Pen(hoverColor), p.X, p.Y, selectionSize.Width - 1, selectionSize.Height - 1);
                    }
                }
            }
        }

        public Point GetIndexPoint(int i)
        {
            int width = (canvas.Width / selectionSize.Width);
            int height = (canvas.Height / selectionSize.Height);
            int x = i % width;
            int y = i / width;
            return new Point(x * selectionSize.Width, y * selectionSize.Height);
        }

        private void GridBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X < 0 || e.Y < 0 || e.X >= canvas.Width || e.Y >= canvas.Height)
            {
                if (hoverIndex != -1)
                {
                    hoverIndex = -1;
                    lastHoverIndex = -1;
                    Invalidate();
                }
                return;
            }

            int width = (canvas.Width / selectionSize.Width);
            int height = (canvas.Height / selectionSize.Height);
            int x = e.X / selectionSize.Width;
            int y = e.Y / selectionSize.Height;
            hoverIndex = x + y * width;

            if (allowMultiSelection && startSelection != -1)
            {
                var startX = startSelection % width;
                var startY = (startSelection - startX) / width;
                var sizeX = x - startX; // same tile = 0; tile left = -1; tile right = 1;
                var sizeY = y - startY; // same tile = 0;
                if(sizeX < 0)
                {
                    startX = x;
                    sizeX *= -1;
                }
                if(sizeY < 0)
                {
                    startY = y;
                    sizeY *= -1;
                }
                sizeX += 1;
                sizeY += 1;

                selectionRectangle.X = startX;
                selectionRectangle.Y = startY;
                selectionRectangle.Width = sizeX;
                selectionRectangle.Height = sizeY;

                /*if (canHover)
                {
                    if (lastHoverIndex != hoverIndex)
                    {
                        lastHoverIndex = hoverIndex;
                        Invalidate();
                    }
                }*/
                Invalidate();
                return;
            }

            if (canHover)
            {
                if (lastHoverIndex != hoverIndex)
                {
                    lastHoverIndex = hoverIndex;
                    Invalidate();
                }
            }
        }

        private void GridBox_MouseLeave(object sender, EventArgs e)
        {
            if (canHover)
            {
                hoverIndex = -1;
                lastHoverIndex = -1;
                Invalidate();
            }
        }

        private void GridBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (canSelect && hoverIndex != -1)
                {
                    selectedIndex = hoverIndex;
                    Invalidate();
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                if (canSelect && hoverIndex != -1)
                {
                    startSelection = hoverIndex;
                    selectionRectangle = new Rectangle((e.X / selectionSize.Width), (e.Y / selectionSize.Height), 1, 1);
                    selectedIndex = hoverIndex;
                    Invalidate();
                }
            }
        }

        private void GridBox_MouseUp(object sender, MouseEventArgs e)
        {
            startSelection = -1;
        }

        private void GridBox_Resize(object sender, EventArgs e)
        {
            if (Image != null)
            {
                canvas.Width = Image.Width;
                canvas.Height = Image.Height;
                Invalidate();
            }
        }

        public void AddMarker(int id, Object data, Func<Object, Tuple<Point[], Brush>> pixelFunc)
        {
            markerDrawers.Add(id, new Tuple<Object, Func<Object, Tuple<Point[], Brush>>>(data, pixelFunc));
            Invalidate();
        }

        public void RemoveMarker(int id)
        {
            markerDrawers.Remove(id);
            Invalidate();
        }

        public void SetScale(int scale)
        {
            if(scale<=0)
            {
                scale = 1;
            }
            this.scale = scale;
            this.selectionSize = new Size(16 * scale, 16 * scale);
            //this.Invalidate(getViewport());
        }

        public new void Invalidate()
        {
            base.Invalidate();
        }
    }
}
