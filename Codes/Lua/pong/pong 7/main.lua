Class = require 'class'
push = require 'push'

require 'Ball'
require 'Paddle'

WINDOW_WIDTH = 1280
WINDOW_HEIGHT = 720

VIRTUAL_WIDTH = 432
VIRTUAL_HEIGHT = 243

PADDLE_SPEED = 200

function love.load()
	math.randomseed(os.time())
	love.graphics.setDefaultFilter('nearest','nearest')

	smallFont = love.graphics.newFont('font.TTF', 8)

	scoreFont = love.graphics.newFont('font.TTF', 32)

	player1Score = 0
	player2Score = 0

	paddle1 = Paddle(5, 30, 5, 20)
	paddle2 = Paddle(VIRTUAL_WIDTH - 10, VIRTUAL_HEIGHT - 50, 5, 20)

	ball = Ball(VIRTUAL_WIDTH / 2 - 2,  VIRTUAL_HEIGHT / 2 - 2, 4, 4)

	gameState = 'start'

	push:setupScreen(VIRTUAL_WIDTH, VIRTUAL_HEIGHT, WINDOW_WIDTH, WINDOW_HEIGHT,{
		fullscreen = false,
		vsync = true,
		resizable = false
	})
end

function love.update(dt)

	paddle1:update(dt)
	paddle2:update(dt)

	if ball:collides(paddle1) then
		ball.dx = -ball.dx
	end

	if ball:collides(paddle2) then
		ball.dx = -ball.dx
	end

	if ball.y <= 0 then
		ball.dy = -ball.dy
		ball.y = 0
	end

	if ball.y >= VIRTUAL_HEIGHT - 4 then
		ball.dy = -ball.dy
		ball.y = VIRTUAL_HEIGHT - 4
	end

	if love.keyboard.isDown('w') then
		paddle1.dy = -PADDLE_SPEED
	elseif love.keyboard.isDown('s') then
		paddle1.dy = PADDLE_SPEED
	else 
		paddle1.dy = 0
	end

	if love.keyboard.isDown('up') then
		paddle2.dy = -PADDLE_SPEED
	elseif love.keyboard.isDown('down') then
		paddle2.dy = PADDLE_SPEED
	else
		paddle2.dy = 0
	end

	if gameState == 'play' then
		ball:update(dt)
	end
end


function love.keypressed(key)
	if key == 'escape' then
		love.event.quit()
	elseif key == 'enter' or key == 'return' then
		if gameState == 'start' then
			gameState = 'play' 
		elseif gameState == 'play' then
			gameState = 'start'
			ball:reset()
		end
	end
end

function love.draw()

	push:apply('start')
	
	love.graphics.clear(40/255, 45/255, 52/255, 255/255)

	love.graphics.setFont(smallFont)
	if gameState == 'start' then
		love.graphics.printf("Hello Pong!", 0, 20, VIRTUAL_WIDTH, 'center')
	end

	if gameState == 'play' then
		love.graphics.printf("Play State!", 0, 20, VIRTUAL_WIDTH, 'center')
	end

	paddle1:render()
	paddle2:render()

	ball:render()

	displayFPS()

	love.graphics.setFont(scoreFont)
	love.graphics.print(player1Score, VIRTUAL_WIDTH / 2 - 50, VIRTUAL_HEIGHT / 3)
	love.graphics.print(player2Score, VIRTUAL_WIDTH / 2 + 30, VIRTUAL_HEIGHT / 3)

	push:apply('end')
end

function displayFPS()
    love.graphics.setFont(smallFont)
    love.graphics.setColor(0, 255/255, 0, 255/255)
    love.graphics.print('FPS: ' .. tostring(love.timer.getFPS()), 40, 20)
    love.graphics.setColor(1, 1, 1, 1)
end
