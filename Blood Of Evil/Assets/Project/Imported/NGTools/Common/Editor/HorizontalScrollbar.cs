using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public sealed class HorizontalScrollbar
	{
		private sealed class PointOfInterest
		{
			public float offset;
			public Color color;
		}

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

		public bool	hasCustomArea = false;
		public Rect	allowedMouseArea;

		public float	interestSizeMargin = 4F;

		private float	scrollX;
		private float	scrollWidth;
		private Rect	bgScrollRect;
		private float	onDownXOffset;

		private List<PointOfInterest>	pointsOfInterest = new List<PointOfInterest>();

		// Cache variable
		private Rect	scrollRect = default(Rect);
		private Color	focused;
		private Color	idle;
		private Color	currentBackgroundColor;

		public	HorizontalScrollbar(float x, float y, float width) : this(x, y, width, 15F, 4F)
		{
		}

		public	HorizontalScrollbar(float x, float y, float width, float height) : this(x, y, width, height, 4F)
		{
		}

		public	HorizontalScrollbar(float x, float y, float width, float height, float innerMargin)
		{
			this.innerMargin = innerMargin;

			this.idle = EditorGUIUtility.isProSkin == true ? new Color(.6F, .6F, .6F) : new Color(.4F, .4F, .4F);
			this.focused = EditorGUIUtility.isProSkin == true ? new Color(.7F, .7F, .7F) : new Color(.3F, .3F, .3F);
			this.currentBackgroundColor = this.idle;

			this.bgScrollRect = new Rect(x, y, width, height);
		}

		/// <summary>
		/// Draw the scrollbar and update offsets.
		/// </summary>
		public void	OnGUI()
		{
			// Toggle scrollbar
			if (Event.current.type == EventType.Repaint &&
				this.bgScrollRect.width >= this.realWidth)
			{
				this.offsetX = 0F;
				this.DrawInterest();
				return;
			}

			if (Event.current.type != EventType.Repaint &&
				this.interceiptEvent == false &&
				bgScrollRect.Contains(Event.current.mousePosition) == false &&
				(this.hasCustomArea == false ||
				 this.allowedMouseArea.Contains(Event.current.mousePosition) == false) &&
				 onDownXOffset == -1F)
			{
				return;
			}

			switch (Event.current.type)
			{
				case EventType.scrollWheel:
					this.scrollX += Event.current.delta.y * this.speedScroll * this.bgScrollRect.width / this.__realWidth;
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
						}
						else
						{
							this.onDownXOffset = this.bgScrollRect.x + this.scrollWidth * .5F;
							this.scrollX = Event.current.mousePosition.y - this.onDownXOffset;
							this.UpdateOffset();
						}

						this.currentBackgroundColor = this.focused;
						HandleUtility.Repaint();
						Event.current.Use();
					}
					break;

				case EventType.MouseMove:
				case EventType.mouseUp:
					this.currentBackgroundColor = this.idle;
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
				this.scrollRect.yMin += this.innerMargin;
				this.scrollRect.yMax -= this.innerMargin;
			}
			EditorGUI.DrawRect(this.scrollRect, this.currentBackgroundColor);

			this.DrawInterest();
		}

		private void	DrawInterest()
		{
			this.scrollRect.y = this.bgScrollRect.y + this.interestSizeMargin * .5F;
			this.scrollRect.width = this.interestSizeMargin;
			this.scrollRect.height = this.bgScrollRect.height - this.interestSizeMargin;

			if (this.innerMargin != 0F)
			{
				this.scrollRect.yMin += this.innerMargin;
				this.scrollRect.yMax -= this.innerMargin;
			}

			float	min = this.bgScrollRect.x - this.scrollRect.width * .5F;

			if (this.bgScrollRect.height <= this.__realWidth)
			{
				float	factor = this.bgScrollRect.width / this.__realWidth;

				for (int i = 0; i < this.pointsOfInterest.Count; i++)
				{
					this.scrollRect.x = min + this.pointsOfInterest[i].offset * factor;
					EditorGUI.DrawRect(this.scrollRect, this.pointsOfInterest[i].color);
				}
			}
			else
			{
				for (int i = 0; i < this.pointsOfInterest.Count; i++)
				{
					this.scrollRect.x = min + this.pointsOfInterest[i].offset;
					EditorGUI.DrawRect(this.scrollRect, this.pointsOfInterest[i].color);
				}
			}
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
				this.scrollWidth = this.bgScrollRect.width * this.bgScrollRect.width / this.__realWidth;
				this.UpdateOffset();
			}
		}

		public void	AddInterest(float offset, Color color)
		{
			this.pointsOfInterest.Add(new PointOfInterest() { offset = offset, color = color });
		}

		public void	ClearInterests()
		{
			this.pointsOfInterest.Clear();
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
							   (this.__realWidth - this.bgScrollRect.width);
		}
	}
}