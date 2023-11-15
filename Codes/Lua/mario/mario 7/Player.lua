Player = Class{}
require 'Animation'

local MOVE_SPEED = 140
local JUMP_VELOCITY = 400
local GRAVITY = 40

function Player:init(map)
    self.width = 16
    self.height = 20

    self.x = map.tileWidth * 10
    self.y = map.tileHeight * (map.mapHeight/2 - 1) - self.height
    
    self.dx = 0
    self.dy = 0

    self.map = map
    self.texture = love.graphics.newImage('graphics/blue_alien.png')
    self.frames = generateQuads(self.texture, 16, 20)
    self.currentFrame = nil

    self.state = 'idle'
    self.direction = 'right'

    self.animations = {
        ['idle'] = Animation{
            texture = self.texture,
            frames = {
                self.frames[1]
            },
            interval = 1
        },
        ['walking'] = Animation{
            texture = self.texture,
            frames = {
                self.frames[8],self.frames[9],self.frames[10],self.frames[11],
            },
            interval = 0.15
        },
        ['jumping'] = Animation{
            texture = self.texture,
            frames = {
                self.frames[3]
            },
            interval = 1
        }
    }

    self.animation = self.animations['idle']
    self.currentFrame = self.animation:getCurrentFrame()

    self.behaviors = {
        ['idle'] = function(dt)
            if love.keyboard.wasPressed('space') then
                self.dy = -JUMP_VELOCITY
                self.state = 'jumping'
                self.animation = self.animations['jumping']
            elseif love.keyboard.isDown('left') then
                self.dx = -MOVE_SPEED
                self.animation = self.animations['walking']
                self.direction = 'left'
            elseif love.keyboard.isDown('right') then
                self.dx = MOVE_SPEED
                self.animation = self.animations['walking']
                self.direction = 'right'
            else
                self.dx = 0
                self.state = 'idle'
                self.animation = self.animations['idle']
            end
        end,

        ['walking'] = function(dt)
            if love.keyboard.wasPressed('space') then
                self.dy = -JUMP_VELOCITY
                self.state = 'jumping'
                self.animation = self.animations['jumping']
            elseif love.keyboard.isDown('left') then
                self.dx = -MOVE_SPEED
                self.animation = self.animations['walking']
                self.direction = 'left'
            elseif love.keyboard.isDown('right') then
                self.dx = MOVE_SPEED
                self.animation = self.animations['walking']
                self.direction = 'right'
            else
                self.dx = 0
                self.state = 'idle'
                self.animation = self.animations['idle']
            end
        end,

        ['jumping'] = function(dt)
            if self.y > 300 then
                return
            end
            
            if love.keyboard.isDown('left') then
                self.direction = 'left'
                self.dx = -MOVE_SPEED
            elseif love.keyboard.isDown('right') then
                self.direction = 'right'
                self.dx = MOVE_SPEED
            end

            self.dy = self.dy + GRAVITY

            if self.y >= map.tileHeight * (map.mapHeight/2 - 1) - self.height then 
                self.y = map.tileHeight * (map.mapHeight/2 - 1) - self.height
                self.dy = 0
                self.state = 'idle'
                self.animation = self.animations[self.state]    
            end
        end
    }
end

function Player:update(dt)
    self.behaviors[self.state](dt)
    self.animation:update(dt)
    self.currentFrame = self.animation:getCurrentFrame()
    self.x = self.x + self.dx * dt

    if self.dy < 0 then
        if self.map:tileAt(self.x, self.y) ~= TILE_EMPTY or
            self.map:tileAt(self.x + self.width - 1, self.y) ~= TILE_EMPTY then
            self.dy = 0

            if self.map:tileAt(self.x, self.y) == JUMP_BLOCK then
                self.map:setTile(math.floor(self.x / self.map.tileWidth) + 1,
                    math.floor(self.y / self.map.tileHeight) + 1, JUMP_BLOCK_HIT)
            end
            if self.map:tileAt(self.x + self.width - 1, self.y) == JUMP_BLOCK then
                self.map:setTile(math.floor((self.x + self.width - 1) / self.map.tileWidth) + 1,
                    math.floor(self.y / self.map.tileHeight) + 1, JUMP_BLOCK_HIT)
            end
        end
    end

    self.y = self.y + self.dy * dt

    --self.y = math.min(self.y + self.dy * dt, 
            --self.map.tileHeight*((self.map.mapHeight-2)/2) - self.height)
end

function Player:render()
    local scaleX

    if self.direction == 'right' then
        scaleX = 1
    else
        scaleX = -1
    end

    love.graphics.draw(self.texture, self.animation:getCurrentFrame(), 
        math.floor(self.x + self.width/2), math.floor(self.y + self.height/2),
        0, scaleX, 1,
        self.width/2, self.height/2)
end