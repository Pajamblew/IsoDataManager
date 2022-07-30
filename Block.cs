// Decompiled with JetBrains decompiler
// Type: IsoDataManager.Block
// Assembly: IsoDataManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C7DAEF67-3FCE-4F74-9EA7-9C78771F8F42
// Assembly location: C:\Users\E1219903\Program\AutoCad\Use\Pentair\IsoDataManager\IsoDataManager.dll

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace IsoDataManager
{
	public class Block : IEquatable<Block>
	{
		protected virtual Type EqualityContract
		{
			
			[CompilerGenerated]
			get
			{
				return typeof(Block);
			}
		}

		public string Name;


		public Dictionary<string, string> Attributes;
		

		public Block(string Name, Dictionary<string, string> Attributes)
		{
			this.Name = Name;
			this.Attributes = Attributes;
			//base._002Ector();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Block");
			stringBuilder.Append(" { ");
			if (PrintMembers(stringBuilder))
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		protected virtual bool PrintMembers(StringBuilder builder)
		{
#nullable enable
			builder.Append("Name");
			builder.Append(" = ");
			builder.Append((object?)Name);
			builder.Append(", ");
			builder.Append("Attributes");
			builder.Append(" = ");
			builder.Append(Attributes);
			return true;
		}


		public static bool operator !=(Block? left, Block? right)
		{
			return !(left == right);
		}

		
		public static bool operator ==(Block? left, Block? right)
		{
#nullable disable
			if ((object)left != right)
			{
				return left?.Equals(right) ?? false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return (EqualityComparer<Type>.Default.GetHashCode(EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name)) * -1521134295 + EqualityComparer<Dictionary<string, string>>.Default.GetHashCode(Attributes);
		}

#nullable enable
		public override bool Equals(object? obj)
		{
			return Equals(obj as Block);
		}

		public virtual bool Equals(Block? other)
		{
#nullable disable
			if ((object)this != other)
			{
				if ((object)other != null && EqualityContract == other!.EqualityContract && EqualityComparer<string>.Default.Equals(Name, other!.Name))
				{
					return EqualityComparer<Dictionary<string, string>>.Default.Equals(Attributes, other!.Attributes);
				}
				return false;
			}
			return true;
		}

		public virtual Block _003CClone_003E_0024()
		{
			return new Block(this);
		}

		protected Block(Block original)
		{
			Name = original.Name;
			Attributes = original.Attributes;
		}

		public void Deconstruct(out string Name, out Dictionary<string, string> Attributes)
		{
			Name = this.Name;
			Attributes = this.Attributes;
		}
	}
}
