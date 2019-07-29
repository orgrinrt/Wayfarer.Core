extends Node
class_name Log
tool

func _ready():
	pass
	
func Print(var string : String, var gdPrint : bool = false):
	var csLog = get_node("/root/CS");
	csLog.call("Print", string, gdPrint);
	pass
