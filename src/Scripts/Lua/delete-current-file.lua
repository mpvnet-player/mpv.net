
-- Only supported on Windows.

-- This script deletes the file that is currently playing
-- via keyboard shortcut, the file is moved to the recycle bin.

-- Usage:
-- Configure input.conf as described below.
-- Press 0 to initiate the delete operation.
-- Press 1 to confirm and delete.

-- input.conf:

-- KP0 script-binding delete_current_file/delete
--   0 script-binding delete_current_file/delete

-- KP1 script-binding delete_current_file/confirm
--   1 script-binding delete_current_file/confirm

function delete()
    file_to_delete = mp.get_property("path")
    delete_time = os.time()
    mp.commandv("show-text", "Press 1 to delete file", "10000")
end

function confirm()
    local path = mp.get_property("path")

    if file_to_delete == path and (os.time() - delete_time) < 10 then
        mp.commandv("show-text", "")

        local count = mp.get_property_number("playlist-count")
        local pos   = mp.get_property_number("playlist-pos")
        local new_pos = 0

        if pos == count - 1 then
            new_pos = pos - 1
        else
            new_pos = pos + 1
        end

        if new_pos > -1 then
            mp.command("set pause no")
            mp.set_property_number("playlist-pos", new_pos)
        end

        mp.command("playlist-remove " .. pos)

        local ps_code = [[& {
            Start-Sleep -Seconds 2
            Add-Type -AssemblyName Microsoft.VisualBasic
            [Microsoft.VisualBasic.FileIO.FileSystem]::DeleteFile('file_to_delete', 'OnlyErrorDialogs', 'SendToRecycleBin')
        }]]

        local escaped_file_to_delete = string.gsub(file_to_delete, "'", "''")
        escaped_file_to_delete = string.gsub(escaped_file_to_delete, "’", "’’")
        escaped_file_to_delete = string.gsub(escaped_file_to_delete, "%%", "%%%%")
        ps_code = string.gsub(ps_code, "file_to_delete", escaped_file_to_delete)

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
