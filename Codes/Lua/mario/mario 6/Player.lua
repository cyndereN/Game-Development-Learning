Player = Class{}
require 'Animation'

local MOVE_SPEED = 80
local WALKING_SPEED = 140
local JUMP_VELOCITY = 400

function Player:init(map)
    self.width = 16
    self.height = 20

    self.x = map.tileWidth * 10
    self.y = map.tileHeight * (map.mapHeight/2 - 1) - self.height
    
    self.texture = love.graphics.newImage('graphics/blue_alien.png')
    self.frames = generateQuads(self.texture, 16, 20)

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

    }

    self.animation = self.animations['idle']
    self.currentFrame = self.animation:getCurrentFrame()

    self.behaviors = {
        ['idle'] = function(dt)
            if love.keyboard.isDown('left') then
                self.x = self.x - MOVE_SPEED * dt
                self.animation = self.animations['walking']
                self.direction = 'left'
            elseif love.keyboard.isDown('right') then
                self.x = self.x + MOVE_SPEED * dt
                self.animation = self.animations['walking']
                self.direction = 'right'
            else
                self.animation = self.animations['idle']
            end
        end,

        ['walking'] = function(dt)
            if love.keyboard.isDown('left') then
                self.x = self.x - MOVE_SPEED * dt
                self.animation = self.animations['walking']
                self.direction = 'left'
            elseif love.keyboard.isDown('right') then
                self.x = self.x + MOVE_SPEED * dt
                self.animation = self.animations['walking']
                self.direction = 'right'
            else
                self.animation = self.animations['idle']
            end
        end,

    }
end

function Player:update(dt)
    self.behaviors[self.state](dt)
    self.animation:update(dt)
    self.currentFrame = self.animation:getCurrentFrame()
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