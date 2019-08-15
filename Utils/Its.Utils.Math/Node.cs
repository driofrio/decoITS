using System.Collections.Generic;

namespace Its.Utils.Math
{
	/// <summary>
	/// Node.
	/// </summary>
	public class Node<T,SArc> 
		where T : class
		where SArc : class
	{
		private string _key;
		private T _value;
		private Dictionary<ArcKey,Arc<T,SArc>> _inArcs = new Dictionary<ArcKey, Arc<T,SArc>> ();
		private Dictionary<ArcKey,Arc<T,SArc>> _outArcs = new Dictionary<ArcKey, Arc<T,SArc>> ();

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.Utils.Math.Node`2"/> class.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="description">Description.</param>
		/// <param name="specification">Specification of the template.</param>
		public Node(string key, string description, T specification){
			this._key=key;
			this._value = specification;
		}

		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <value>The key.</value>
		public string Key{
			get{
				return _key;
			}
		}

		/// <summary>
		/// Gets the specification.
		/// </summary>
		/// <value>The specification.</value>
		public T Specification{
			get{
				return _value;
			}
		}

		/// <summary>
		/// Gets the arcs that arrives to this node.
		/// </summary>
		/// <value>The arriving arcs.</value>
		public Dictionary<ArcKey,Arc<T,SArc>> InArcs{
			get{
				return _inArcs;
			}
		}

		/// <summary>
		/// Gets the arcs departing from this node.
		/// </summary>
		/// <value>The departing arcs.</value>
		public Dictionary<ArcKey,Arc<T,SArc>> OutArcs{
			get{
				return _outArcs;
			}
		}

		public void AddInArc(Arc<T,SArc> arc){
			if (!_inArcs.ContainsValue (arc)) {
				_inArcs.Add (arc.Key, arc);
			}
		}

		/// <summary>
		/// Adds the an arc that departs from this node.
		/// </summary>
		/// <param name="arc">Arc.</param>
		public void AddOutArc(Arc<T,SArc> arc){
			if (!_outArcs.ContainsValue (arc)) {
				_outArcs.Add (arc.Key, arc);
			}
		}

		/// <summary>
		/// Removes an inArc.
		/// </summary>
		/// <param name="arc">Arc.</param>
		public void RemoveInArc(Arc<T,SArc> arc){
			if (!_inArcs.ContainsValue (arc)) {
				_inArcs.Remove (arc.Key);
			}
		}

		/// <summary>
		/// Removes the outArc.
		/// </summary>
		/// <param name="arc">Arc.</param>
		public void RemoveOutArc(Arc<T,SArc> arc){
			if (!_outArcs.ContainsValue (arc)) {
				_outArcs.Remove (arc.Key);
			}
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Its.Utils.Math.Node`2"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Its.Utils.Math.Node`2"/>.</returns>
		public override string ToString(){
			return "Key: " + _key + " " + _value.ToString ();
		}
	}
}