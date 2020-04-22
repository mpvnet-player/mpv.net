
local did_minimize = false

mp.observe_property("window-minimized", "bool", function(name, value)
    local pause = mp.get_property_bool("pause")

    if value == true then
        if pause == false then
            mp.set_property_bool("pause", true)
            did_minimize = true
        end
    elseif value == false then
        if did_minimize and (pause == true) then
            mp.set_property_bool("pause", false)
        end

        did_minimize = false
    end
end)
