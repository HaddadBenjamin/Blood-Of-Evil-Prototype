using UnityEngine;
using System.Collections;
using UnityEditor;

namespace NGToolsEditor
{
	public class HorizontalScrollbar
	{
		public float	innerMargin;
		public float	speedScroll = 10F;
		private float	__realWidth;
		public float	realWidth
		{
			get
			{
				return this.__realWidth;
			}
			set
			{
				if (this.__realWidth != value)
				{
					this.__realWidth = value;
					this.scrollWidth = this.bgScrollRect.width * this.bgScrollRect.width / this.__realWidth;
					this.UpdateOffset();
				}
			}
		}
		public float	offsetX { get; private set; }
		public float	maxHeight
		{
			get
			{
				return (this.bgScrollRect.width < this.realWidth) ? this.bgScrollRect.height : 0F;
			}
		}
		public float	maxWidth { get { return this.bgScrollRect.width; } }
		public bool		interceiptEvent = true;

		private float	scrollX;
		private float	scrollWidth;
		private Rect	bgScrollRect;
		private float	onDownXOffset;

		// Cache variable
		private GUIStyle	scrollStyle;
		private Rect		scrollRect = default(Rect);
		private Texture2D	focused;
		private Texture2D	idle;

		public	HorizontalScrollbar(float x, float y, float width) : this(x, y, width, 15F, 4F)
		{
		}

		public	HorizontalScrollbar(float x, float y, float width, float height) : this(x, y, width, height, 4F)
		{
		}

		public	HorizontalScrollbar(float x, float y, float width, float height, float innerMargin)
		{
			this.innerMargin = innerMargin;

			this.focused = new Texture2D(1, 1);
			this.focused.SetPixel(0, 0, new Color(.5F, .5F, .5F));
			this.focused.Apply();
			this.idle = new Texture2D(1, 1);
			this.idle.SetPixel(0, 0, new Color(.7F, .7F, .7F));
			this.idle.Apply();

			this.scrollStyle = new GUIStyle();
			this.scrollStyle.normal.background = this.idle;
			this.scrollStyle.focused.background = this.focused;

			this.bgScrollRect = new Rect(x, y, width, height);
		}

		/// <summary>
		/// Draw the scrollbar and update offsets.
		/// </summary>
		public void	OnGUI()
		{
			// Toggle scrollbar
			if (this.bgScrollRect.width >= this.realWidth)
			{
				this.offsetX = 0f;
				return;
			}

			if (Event.current.type != EventType.Repaint &&
				this.interceiptEvent == false &&
				bgScrollRect.Contains(Event.current.mousePosition) == false)
			{
				return;
			}

			switch (Event.current.type)
			{
				case EventType.scrollWheel:
					this.scrollX += Event.current.delta.y * this.speedScroll;
					this.UpdateOffset();
					HandleUtility.Repaint();
					Event.current.Use();
					break;

				case EventType.mouseDrag:
					if (this.onDownXOffset > 0F)
					{
						this.scrollX = Event.current.mousePosition.x - this.onDownXOffset;
						this.UpdateOffset();
						HandleUtility.Repaint();
					}
					break;

				case EventType.mouseDown:
					if (this.bgScrollRect.Contains(Event.current.mousePosition) == true)
					{
						if (Event.current.mousePosition.x >= this.bgScrollRect.x + this.scrollX &&
							Event.current.mousePosition.x < this.bgScrollRect.x + this.scrollX + this.scrollWidth)
						{
							this.onDownXOffset = Event.current.mousePosition.x - this.scrollX;
							this.scrollStyle.normal.background = EditorGUIUtility.whiteTexture;
							HandleUtility.Repaint();
						}
						else
						{
							this.onDownXOffset = this.scrollWidth * .5F;
							this.scrollStyle.normal.background = EditorGUIUtility.whiteTexture;

							this.scrollX = Event.current.mousePosition.x * 100F / this.bgScrollRect.width *
										   this.bgScrollRect.width / 100F - this.onDownXOffset;
							this.UpdateOffset();
							HandleUtility.Repaint();
						}

						Event.current.Use();
					}
					break;

				case EventType.mouseUp:
					this.scrollStyle.normal.background = this.idle;
					this.onDownXOffset = -1;
					HandleUtility.Repaint();
					break;

				default:
					break;
			}
			this.scrollRect.x = this.bgScrollRect.x + this.scrollX;
			this.scrollRect.y = this.bgScrollRect.y;
			this.scrollRect.width = this.scrollWidth;
			this.scrollRect.height = this.bgScrollRect.height;
			if (this.innerMargin != 0F)
			{
				this.scrollRect.xMin += this.innerMargin;
				this.scrollRect.xMax -= this.innerMargin;
				this.scrollRect.yMin += this.innerMargin;
				this.scrollRect.yMax -= this.innerMargin;
			}
			EditorGUI.LabelField(this.scrollRect, "", this.scrollStyle);
		}

		public void	SetPosition(float x, float y)
		{
			if (Event.current.type != EventType.Layout &&
				(this.bgScrollRect.x != x ||
				 this.bgScrollRect.y != y))
			{
				this.bgScrollRect.x = x;
				this.bgScrollRect.y = y;
			}
		}

		public void	SetSize(float width)
		{
			if (Event.current.type != EventType.Layout &&
				this.bgScrollRect.width != width)
			{
				this.bgScrollRect.width = width;
				// Update width, function of the max content width
				this.scrollWidth = this.bgScrollRect.width * this.bgScrollRect.width / this.realWidth;
				this.UpdateOffset();
			}
		}

		private void	UpdateOffset()
		{
			if (this.scrollX < 0F)
				this.scrollX = 0F;
			else if (this.scrollX + this.scrollWidth > this.bgScrollRect.width)
				this.scrollX = this.bgScrollRect.width - this.scrollWidth;

			if (this.scrollX <= 0F)
				this.offsetX = 0F;
			else
				this.offsetX = (this.scrollX / (this.bgScrollRect.width - this.scrollWidth)) *
							   (this.realWidth - this.bgScrollRect.width);
		}
	}
}