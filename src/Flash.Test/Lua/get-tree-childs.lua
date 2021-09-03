local function getChild(currentnode, t, res)
  if currentnode == nil or t == nil  then
    return res
  end

  local nextNode = nil
  local nextType = nil
  if t == "RoleId" and (type(currentnode) == "number" or type(currentnode) == "string") then
    local treeNode = redis.call("HGET", @CacheKey, currentnode)
    if treeNode then
      local node = cjson.decode(treeNode)
      table.insert(res, treeNode)
      if node and node.ChildIds then
        nextNode = node.ChildIds
        nextType = "ChildIds"
      end
    end
  elseif t == "ChildIds" then
    nextNode = {}
    nextType = "ChildIds"
    local treeNode  = nil
    local node = nil
    local cnt = 0
    for _, val in ipairs(currentnode) do
      treeNode = redis.call("HGET", @CacheKey, tostring(val))
      if treeNode then
        node = cjson.decode(treeNode)
        table.insert(res, treeNode)
        if node and node.ChildIds then
          for _, val2 in ipairs(node.ChildIds) do
            table.insert(nextNode, val2)
            cnt = cnt + 1
          end
        end
      end
    end
    if cnt == 0 then
      nextNode = nil
      nextType = nil
    end
  end
  return getChild(nextNode, nextType, res)
end


if @CacheKey and @DataKey then
  return getChild(@DataKey, "RoleId", {})
end

return {}