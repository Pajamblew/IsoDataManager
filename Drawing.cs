using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace IsoDataManager
{
	public class Drawing : IEquatable<Drawing>
	{
		protected virtual Type EqualityContract
		{
			
			[CompilerGenerated]
			get
			{
				return typeof(Drawing);
			}
		}

		public string Path;
		
		public Dictionary<string, List<Block>> Blocks;
		
		public Drawing(string Path, Dictionary<string, List<Block>> Blocks)
		{
			this.Path = Path;
			this.Blocks = Blocks;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Drawing");
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
			builder.Append("Path");
			builder.Append(" = ");
			builder.Append((object?)Path);
			builder.Append(", ");
			builder.Append("Blocks");
			builder.Append(" = ");
			builder.Append(Blocks);
			return true;
		}
		public static bool operator !=(Drawing? left, Drawing? right)
		{
			return !(left == right);
		}

		
		public static bool operator ==(Drawing? left, Drawing? right)
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
			return (EqualityComparer<Type>.Default.GetHashCode(EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Path)) * -1521134295 + EqualityComparer<Dictionary<string, List<Block>>>.Default.GetHashCode(Blocks);
		}
#nullable enable
		public override bool Equals(object? obj)
		{
			return Equals(obj as Drawing);
		}

#nullable enable
		public virtual bool Equals(Drawing? other)
		{
#nullable disable
			if ((object)this != other)
			{
				if ((object)other != null && EqualityContract == other!.EqualityContract && EqualityComparer<string>.Default.Equals(Path, other!.Path))
				{
					return EqualityComparer<Dictionary<string, List<Block>>>.Default.Equals(Blocks, other!.Blocks);
				}
				return false;
			}
			return true;
		}

		public virtual Drawing _003CClone_003E_0024()
		{
			return new Drawing(this);
		}

		protected Drawing(Drawing original)
		{
			Path = original.Path;
			Blocks = original.Blocks;
		}

		public void Deconstruct(out string Path, out Dictionary<string, List<Block>> Blocks)
		{
			Path = this.Path;
			Blocks = this.Blocks;
		}
	}
}
