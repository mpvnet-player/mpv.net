
-- https://github.com/unifiedremote/Docs/blob/master/libs/keyboard.md

-- https://github.com/unifiedremote/Docs/blob/master/res/keys.md

local kb = libs.keyboard;

actions.play = function ()
	kb.stroke("space");
end

actions.next = function ()
	kb.stroke("F12");
end

actions.prev = function ()
	kb.stroke("F11");
end

actions.enter = function ()
	kb.stroke("enter");
end

actions.left = function ()
	kb.stroke("left");
end

actions.right = function ()
	kb.stroke("right");
end

actions.up = function ()
	kb.stroke("up");
end

actions.down = function ()
	kb.stroke("down");
end

actions.vol_minus = function ()
	kb.text("-");
end

actions.vol_plus = function ()
	kb.text("+");
end

actions.mute = function ()
	kb.stroke("m");
end

actions.info = function ()
	kb.stroke("i");
end

actions.KP0 = function ()
	kb.stroke("num0");
end

actions.KP1 = function ()
	kb.stroke("num1");
end

actions.KP2 = function ()
	kb.stroke("num2");
end

actions.KP3 = function ()
	kb.stroke("num3");
end

actions.KP4 = function ()
	kb.stroke("num4");
end

actions.KP5 = function ()
	kb.stroke("num5");
end

actions.KP6 = function ()
	kb.stroke("num6");
end

actions.KP7 = function ()
	kb.stroke("num7");
end

actions.KP8 = function ()
	kb.stroke("num8");
end

actions.KP9 = function ()
	kb.stroke("num9");
end

actions.ab_loop = function ()
	kb.stroke("l");
end

actions.zoom_in = function ()
    kb.stroke("ctrl", "oem_plus");
end

actions.zoom_out = function ()
    kb.stroke("ctrl", "oem_minus");
end
