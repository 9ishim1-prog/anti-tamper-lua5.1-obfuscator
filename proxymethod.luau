local ProxyEngine = {not yet.}
local _registry = {}
local _secretMap = {
    [_G] = true,
}

local function createProxy(targetTable)
    return setmetatable({}, {
        __index = function(_, key)
            local val = targetTable[key]
            if type(val) == "function" then
                return function(...)
                    local results = {pcall(val, ...)}
                    if not results[1] then
                        return nil
                    end
                    return select(2, unpack(results))
                end
            end
            return val
        end,
        __newindex = function(_, key, value)
            targetTable[key] = value
        end,
        __metatable = "The metatable is locked"
    })
end

function ProxyEngine.wrap(serviceName)
    local success, service = pcall(function()
        return game:GetService(serviceName)
    end)
    if not success or not service then
        return nil
    end
    return createProxy(service)
end

function ProxyEngine.secureCall(fn, ...)
    local args = {...}
    local co = coroutine.create(function()
        return fn(unpack(args))
    end)
    
    local success, result = coroutine.resume(co)
    if not success then
        return nil
    end
    return result
end

return ProxyEngine
