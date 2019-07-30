tool
extends Node

static func get_children_recursive(var node : Node) -> Array:
	var cs_log = load("res://Addons/Wayfarer/Core/Gd.cs");
	var cs_instance = cs_log.new();
	var children = cs_instance.call("GetChildrenRecursive", node);
	cs_instance.queue_free();
	return children;