
-- This script deletes the file that is currently playing.

-- input.conf:

-- KP0 script-binding delete_current_file/delete
--   0 script-binding delete_current_file/delete

-- KP1 script-binding delete_current_file/confirm
--   1 script-binding delete_current_file/confirm

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
        local newPos = 0

        if pos == count - 1 then
            newPos = pos - 1
        else
            newPos = pos + 1
        end

        if newPos > -1 then
            mp.command("set pause no")
            mp.set_property_number("playlist-pos", newPos)
        end

        mp.command("playlist-remove " .. pos)

        local ps_code = [[& {
            Start-Sleep -Seconds 2
            Add-Type -AssemblyName Microsoft.VisualBasic
            [Microsoft.VisualBasic.FileIO.FileSystem]::DeleteFile('FileToDelete', 'OnlyErrorDialogs', 'SendToRecycleBin')
        }]]

        local escapedFileToDelete = string.gsub(FileToDelete, "'", "''")
        ps_code = string.gsub(ps_code, "FileToDelete", escapedFileToDelete)

        mp.command_native({
            name = "subprocess",
            playback_only = false,
            detach = true,
            args = { 'powershell', '-NoProfile', '-ExecutionPolicy', 'Bypass', '-Command', ps_code },
        })
    end
end

mp.add_key_binding(nil, "delete",  delete)
mp.add_key_binding(nil, "confirm", confirm)
