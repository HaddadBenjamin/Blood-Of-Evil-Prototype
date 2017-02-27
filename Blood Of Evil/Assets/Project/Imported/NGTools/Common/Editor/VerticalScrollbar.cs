using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public sealed class VerticalScrollbar
	{
		private sealed class PointOfInterest
		{
			public float offset;
			public Color color;
		}

		public float	innerMargin;
		public float	speedScroll = 10F;
		private float	__realHeight;
		public float	realHeight
		{
			get
			{
				return this.__realHeight;
			}
			set
			{
				if (this.__realHeight != value)
				{
					this.__realHeight = value;
					this.scrollHeight = this.bgScrollRect.height * this.bgScrollRect.height / this.__realHeight;
					this.UpdateOffset();
				}
			}
		}
		public float	offsetY { get; private set; }
		public float	maxWidth
		{
			get
			{
				return (this.bgScrollRect.height < this.realHeight) ? this.bgScrollRect.width : 0F;
			}
		}
		public float	maxHeight { get { return this.bgScrollRect.height; } }
		public bool		interceiptEvent = true;

		public bool	hasCustomArea = false;
		public Rect	allowedMouseArea;

		public float	interestSizeMargin = 4F;

		private float	scrollY;
		private float	scrollHeight;
		private Rect	bgScrollRect;
		private float	onDownYOffset;

		private List<PointOfInterest>	pointsOfInterest = new List<PointOfInterest>();

		// Cache variable
		private Rect	scrollRect = default(Rect);
		private Color	focused;
		private Color	idle;
		private Color	currentBackgroundColor;

		public	VerticalScrollbar(float x, float y, float height) : this(x, y, height, 15F, 4F)
		{
		}

		public	VerticalScrollbar(float x, float y, float height, float width) : this(x, y, height, width, 4F)
		{
		}

		public	VerticalScrollbar(float x, float y, float height, float width, float innerMargin)
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
				this.bgScrollRect.height >= this.__realHeight)
			{
				this.offsetY = 0F;
				this.DrawInterest();
				return;
			}

			if (Event.current.type != EventType.Repaint &&
				this.interceiptEvent == false &&
				bgScrollRect.Contains(Event.current.mousePosition) == false &&
				(this.hasCustomArea == false ||
				 this.allowedMouseArea.Contains(Event.current.mousePosition) == false) &&
				 onDownYOffset == -1F)
			{
				return;
			}

			switch (Event.current.type)
			{
				case EventType.ScrollWheel:
					this.scrollY += Event.current.delta.y * this.speedScroll * this.bgScrollRect.height / this.__realHeight;
					this.UpdateOffset();
					HandleUtility.Repaint();
					Event.current.Use();
					break;

				case EventType.MouseDrag:
					if (this.onDownYOffset > 0F)
					{
						this.scrollY = Event.current.mousePosition.y - this.onDownYOffset;
						this.UpdateOffset();
						HandleUtility.Repaint();
					}
					break;

				case EventType.MouseDown:
					if (this.bgScrollRect.Contains(Event.current.mousePosition) == true)
					{
						if (Event.current.mousePosition.y >= this.bgScrollRect.y + this.scrollY &&
							Event.current.mousePosition.y < this.bgScrollRect.y + this.scrollY + this.scrollHeight)
						{
							this.onDownYOffset = Event.current.mousePosition.y - this.scrollY;
						}
						else
						{
							this.onDownYOffset = this.bgScrollRect.y + this.scrollHeight * .5F;
							this.scrollY = Event.current.mousePosition.y - this.onDownYOffset;
							this.UpdateOffset();
						}

						this.currentBackgroundColor = this.focused;
						HandleUtility.Repaint();
						Event.current.Use();
					}
					break;

				case EventType.MouseMove:
				case EventType.MouseUp:
					this.currentBackgroundColor = this.idle;
					this.onDownYOffset = -1F;
					HandleUtility.Repaint();
					break;

				default:
					break;
			}

			this.scrollRect.x = this.bgScrollRect.x;
			this.scrollRect.y = this.bgScrollRect.y + this.scrollY;
			this.scrollRect.width = this.bgScrollRect.width;
			this.scrollRect.height = this.scrollHeight;
			if (this.innerMargin != 0F)
			{
				this.scrollRect.xMin += this.innerMargin;
				this.scrollRect.xMax -= this.innerMargin;
			}
			EditorGUI.DrawRect(this.scrollRect, this.currentBackgroundColor);

			this.DrawInterest();
		}

		private void	DrawInterest()
		{
			this.scrollRect.x = this.bgScrollRect.x + this.interestSizeMargin * .5F;
			this.scrollRect.width = this.bgScrollRect.width - this.interestSizeMargin;
			this.scrollRect.height = this.interestSizeMargin;

			if (this.innerMargin != 0F)
			{
				this.scrollRect.xMin += this.innerMargin;
				this.scrollRect.xMax -= this.innerMargin;
			}

			float	min = this.bgScrollRect.y - this.scrollRect.height * .5F;

			if (this.bgScrollRect.height <= this.__realHeight)
			{
				float	factor = this.bgScrollRect.height / this.__realHeight;

				for (int i = 0; i < this.pointsOfInterest.Count; i++)
				{
					this.scrollRect.y = min + this.pointsOfInterest[i].offset * factor;
					EditorGUI.DrawRect(this.scrollRect, this.pointsOfInterest[i].color);
				}
			}
			else
			{
				for (int i = 0; i < this.pointsOfInterest.Count; i++)
				{
					this.scrollRect.y = min + this.pointsOfInterest[i].offset;
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

		public void	SetSize(float height)
		{
			if (Event.current.type != EventType.Layout &&
				this.bgScrollRect.height != height)
			{
				this.bgScrollRect.height = height;
				// Update height, function of the max content height
				this.scrollHeight = this.bgScrollRect.height * this.bgScrollRect.height / this.__realHeight;
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
			if (this.scrollY < 0F)
				this.scrollY = 0F;
			else if (this.scrollY + this.scrollHeight > this.bgScrollRect.height)
				this.scrollY = this.bgScrollRect.height - this.scrollHeight;

			if (this.scrollY <= 0F)
				this.offsetY = 0F;
			else
				this.offsetY = (this.scrollY / (this.bgScrollRect.height - this.scrollHeight)) *
							   (this.__realHeight - this.bgScrollRect.height);
		}
	}
}