extends Node
class_name Log
tool

func _ready():
	pass
	
static func Print(var string : String, var gdPrint : bool = false):
	var cs_log = load("res://Addons/Wayfarer.Core/Core/Gd.cs");
	var cs_instance = cs_log.new();
	cs_instance.call("Print", string, gdPrint);
	cs_instance.queue_free();
	pass

class Wf:
	static func Print(var string : String, var gdPrint : bool = false):
		var cs_log = load("res://Addons/Wayfarer.Core/Core/Gd.cs");
		var cs_instance = cs_log.new();
		cs_instance.call("PrintWf", string, gdPrint);
		cs_instance.queue_free();
		pass