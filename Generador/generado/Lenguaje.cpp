{
	public class Lenguaje
	{
		public Lenguaje ()
		{
		}
		public void Programa()
		{
			if(Contenido == Numero())
			{
				A();
			}
		}
		private void A()
		{
			match(Tipos.Numero);
			if(Contenido == Numero())
			{
				A();
			}
		}
		private void B()
		{
			A()
			match("z");
		}
		private void D()
		{
			match("a");
			if(Contenido == Numero())
			{
				A();
			}
			else
			{
				if(Contenido == "b")
				{
					match("b");
				}
				else
				{
					match(Tipos.b);
				}
				if(Contenido == "c")
				{
					match("c");
				}
				else
				{
					match(Tipos.c);
				}
				C()
			}
		}
		private void C()
		{
			if(Contenido == Numero())
			{
				A();
			}
			else
			{
				B()
			}
		}
		private void E()
		{
			{
