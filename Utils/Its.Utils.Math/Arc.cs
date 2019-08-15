namespace Its.Utils.Math
{
	/// <summary>
	/// Arc.
	/// </summary>
	public class Arc<TNode,S> 
		where TNode : class
		where S : class
	{

		private ArcKey _key;
		private Node<TNode,S> _nodeIn;
		private Node<TNode,S> _nodeOut;
		private S _value;

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.Utils.Math.Arc`2"/> class.
		/// </summary>
		/// <param name="nodeIn">Node in.</param>
		/// <param name="nodeOut">Node out.</param>
		/// <param name="specification">Specification of the template.</param>
		public Arc(Node<TNode,S> nodeOut, Node<TNode,S> nodeIn, S specification){
			this._nodeIn=nodeIn;
			this._nodeOut=nodeOut;
			this._value = specification;
			this._key = new ArcKey (nodeOut.Key, nodeIn.Key);
		}

		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <value>The key.</value>
		public ArcKey Key{
			get{
				return _key;
			}
		}

		/// <summary>
		/// Gets the node in.
		/// </summary>
		/// <value>The node in.</value>
		public Node<TNode,S> NodeIn{
			get{
				return _nodeIn;
			}
		}

		/// <summary>
		/// Gets the node out.
		/// </summary>
		/// <value>The node out.</value>
		public Node<TNode,S> NodeOut{
			get{
				return _nodeOut;
			}
		}

		/// <summary>
		/// Gets the specification.
		/// </summary>
		/// <value>The specification.</value>
		public S Specification{
			get{
				return _value;
			}
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Its.Utils.Math.Arc`2"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Its.Utils.Math.Arc`2"/>.</returns>
		public override string ToString(){
			return _nodeOut.Key + "==>" + _nodeIn.Key + " ("+_value.ToString()+")";
		}
	}

	/// <summary>
	/// Arc key.
	/// </summary>
	public class ArcKey
	{
		private string _nodeKey1;
		private string _nodeKey2;

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.Utils.Math.ArcKey"/> class.
		/// </summary>
		/// <param name="nodeKey1">Node key1.</param>
		/// <param name="nodeKey2">Node key2.</param>
		public ArcKey(string nodeKey1, string nodeKey2){
			this._nodeKey1 = nodeKey1;
			this._nodeKey2 = nodeKey2;
		}

		/// <summary>
		/// Gets the node key1.
		/// </summary>
		/// <value>The node key1.</value>
		public string NodeKey1{
			get{
				return _nodeKey1;
			}
		}

		/// <summary>
		/// Gets the node key2.
		/// </summary>
		/// <value>The node key2.</value>
		public string NodeKey2{
			get{
				return _nodeKey2;
			}
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Its.Utils.Math.ArcKey"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Its.Utils.Math.ArcKey"/>.</returns>
		override public string ToString(){
			return _nodeKey1 + " " + _nodeKey2;
		}

		public override int GetHashCode()
		{
			if (_nodeKey1 == null) return 0;
			return (_nodeKey1 + " " + _nodeKey2).GetHashCode();
		}

		public override bool Equals(object obj)
		{
			ArcKey other = obj as ArcKey;
			return other != null && other.NodeKey1 == this.NodeKey1 && other.NodeKey2 == this.NodeKey2;
		}
	}
}