
-- This script deletes the file that is currently playing.

-- input.conf:

-- KP0 script-binding delete_current_file/delete
--   0 script-binding delete_current_file/delete
-- KP1 script-binding delete_current_file/confirm
--   1 script-binding delete_current_file/confirm

local utils = require 'mp.utils'

function delete_file()
    local code = [[& {
        Add-Type -AssemblyName Microsoft.VisualBasic
        [Microsoft.VisualBasic.FileIO.FileSystem]::DeleteFile('FileToDelete', 'OnlyErrorDialogs', 'SendToRecycleBin')
    }]]

    code = string.gsub(code, "FileToDelete", FileToDelete)

    utils.subprocess({
        args = { 'powershell', '-NoProfile', '-ExecutionPolicy', 'Bypass', '-Command', code },
        playback_only = false,
    })
end

function delete()
    FileToDelete = mp.get_property("path")
    DeleteTime = os.time()
    mp.commandv("show-text", "Press 1 to delete file", "10000")
end

function confirm()
    local path = mp.get_property("path")

    if FileToDelete == path and (os.time() - DeleteTime) < 10 then
        mp.commandv("show-text", "")

        local count = mp.get_property_number("playlist-count")
        local pos = mp.get_property_number("playlist-pos")

        if pos == count - 1 then
            newPos = pos - 1
        else
            newPos = pos + 1
        end

        if newPos > -1 then
            mp.set_property_number("playlist-pos", newPos)
        end

        mp.command("playlist-remove " .. pos)
        mp.add_timeout(2, delete_file)
    end
end

mp.add_key_binding(nil, "delete",  delete)
mp.add_key_binding(nil, "confirm", confirm)
