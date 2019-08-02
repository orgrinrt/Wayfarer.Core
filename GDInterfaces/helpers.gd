tool
extends Node

static func get_children_recursive(var node : Node) -> Array:
	var cs_bridge = load("res://Addons/Wayfarer.Core/Core/Gd.cs");
	var cs_instance = cs_bridge.new();
	var children = cs_instance.call("GetChildrenRecursive", node);
	cs_instance.queue_free();
	return children;