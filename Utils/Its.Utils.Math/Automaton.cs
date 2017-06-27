using System;
using System.Collections.Generic;

namespace Its.Utils.Math
{
	/// <summary>
	/// Graph.
	/// </summary>
	public class Automaton<TNode,TArc>
		where TNode : class
		where TArc : class
	{
		private Dictionary<string,Node<TNode,TArc>> _nodes;
		private Dictionary<string,Arc<TNode,TArc>> _arcs;

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.Utils.Math.Graph`2"/> class.
		/// </summary>
		public Automaton(){
			this._nodes=new Dictionary<string, Node<TNode,TArc>>();
			this._arcs=new Dictionary<string, Arc<TNode, TArc>>();
		}

		/// <summary>
		/// Gets the nodes.
		/// </summary>
		/// <value>The nodes.</value>
		protected Dictionary<string,Node<TNode,TArc>> Nodes{
			get{
				return _nodes;
			}
		}

		/// <summary>
		/// Gets the arcs.
		/// </summary>
		/// <value>The arcs.</value>
		protected Dictionary<string,Arc<TNode,TArc>> Arcs{
			get{
				return _arcs;
			}
		}

		/// <summary>
		/// Tries to get a node.
		/// </summary>
		/// <returns><c>true</c>, if get node exists, <c>false</c> otherwise.</returns>
		/// <param name="nodeKey">Node key.</param>
		/// <param name="node">Node.</param>
		protected bool TryGetNode(string nodeKey, out Node<TNode,TArc> node){
			return _nodes.TryGetValue (nodeKey, out node);
		}

		/// <summary>
		/// Tries to get an arc.
		/// </summary>
		/// <returns><c>true</c>, if get arc exists, <c>false</c> otherwise.</returns>
		/// <param name="nodeKeyOut">Node key out.</param>
		/// <param name="nodeKeyIn">Node key in.</param>
		/// <param name="arc">Arc.</param>
		protected bool TryGetArc(string nodeKeyOut, string nodeKeyIn, out Arc<TNode,TArc> arc){
			return _arcs.TryGetValue (nodeKeyOut + " " + nodeKeyIn, out arc);
		}

		/// <summary>
		/// Tries the get an arc.
		/// </summary>
		/// <returns><c>true</c>, if get arc was tryed, <c>false</c> otherwise.</returns>
		/// <param name="arcKey">Arc key.</param>
		/// <param name="arc">Arc.</param>
		protected bool TryGetArc(ArcKey arcKey, out Arc<TNode,TArc> arc){
			return _arcs.TryGetValue (arcKey.ToString(), out arc);
		}

		/// <summary>
		/// Adds a node.
		/// </summary>
		/// <param name="newNode">New node.</param>
		protected void AddNode(Node<TNode,TArc> newNode){
			_nodes.Add(newNode.Key, newNode);
		}

		/// <summary>
		/// Removes the node.
		/// </summary>
		/// <param name="node">Node.</param>
		protected void RemoveNode(string nodeKey){
			_nodes.Remove(nodeKey);
		}

		/// <summary>
		/// Adds an arc.
		/// </summary>
		/// <param name="newArc">New arc.</param>
		protected void AddArc(Arc<TNode,TArc> newArc){
			_arcs.Add (newArc.Key.ToString(), newArc);
		}

		/// <summary>
		/// Removes an arc.
		/// </summary>
		/// <param name="nodeInKey">Node in key.</param>
		/// <param name="nodeOutKey">Node out key.</param>
		protected void RemoveArc(string nodeOutKey, string nodeInKey){
			_arcs.Remove (nodeOutKey + " " + nodeInKey);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Its.Utils.Math.Graph`2"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Its.Utils.Math.Graph`2"/>.</returns>
		public override string ToString(){
			string graphString = "";//_value.ToString () + Environment.NewLine;
			foreach (KeyValuePair<string,Node<TNode,TArc>> nodePair in _nodes) {
				graphString += nodePair.Value.ToString () + Environment.NewLine;
			}
			foreach (KeyValuePair<string, Arc<TNode,TArc>> arcPair in _arcs) {
				graphString += arcPair.Value.ToString () + Environment.NewLine;
			}
			return graphString;
		}
	}
}

