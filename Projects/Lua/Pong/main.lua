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

	love.window.setTitle('Pong')

	smallFont = love.graphics.newFont('font.TTF', 8)

	scoreFont = love.graphics.newFont('font.TTF', 32)

	victoryFont = love.graphics.newFont('font.TTF', 24)

    sounds = {
        ['paddle_hit'] = love.audio.newSource('sounds/paddle_hit.wav', 'static'),
        ['score'] = love.audio.newSource('sounds/score.wav', 'static'),
        ['wall_hit'] = love.audio.newSource('sounds/wall_hit.wav', 'static')
    }

	player1Score = 0
	player2Score = 0

	servingPlayer = math.random(2) 

	winningPlayer = 0

	paddle1 = Paddle(5, 30, 5, 20)
	paddle2 = Paddle(VIRTUAL_WIDTH - 10, VIRTUAL_HEIGHT - 50, 5, 20)

	ball = Ball(VIRTUAL_WIDTH / 2 - 2,  VIRTUAL_HEIGHT / 2 - 2, 4, 4)

	if servingPlayer == 1 then
		ball.dx = 100
	else
		ball.dx = -100
	end	

	gameState = 'start'

	push:setupScreen(VIRTUAL_WIDTH, VIRTUAL_HEIGHT, WINDOW_WIDTH, WINDOW_HEIGHT,{
		fullscreen = false,
		vsync = true,
		resizable = true
	})
end

function love.resize(w,h)
	push:resize(w,h)
end

function love.update(dt)

	paddle1:update(dt)
	paddle2:update(dt)

	if ball:collides(paddle1) then
		ball.dx = -ball.dx * 1.03
		sounds['paddle_hit']:play()
	end

	if ball:collides(paddle2) then
		ball.dx = -ball.dx * 1.03
		sounds['paddle_hit']:play()
	end

	if ball.y <= 0 then
		ball.dy = -ball.dy
		ball.y = 0
		sounds['wall_hit']:play()
	end

	if ball.y >= VIRTUAL_HEIGHT - 4 then
		ball.dy = -ball.dy
		ball.y = VIRTUAL_HEIGHT - 4
		sounds['wall_hit']:play()
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

		if ball.x <= 0 then
			player2Score = player2Score + 1
			sounds['score']:play()
			ball:reset()
			ball.dx = 100
			servingPlayer = 1

			if player2Score >= 3 then
				gameState = 'victory'
				winningPlayer = 2
			else
				gameState = 'serve'
			end
		end

		if ball.x > VIRTUAL_WIDTH + 4 then
			player1Score = player1Score + 1
			sounds['score']:play()
			ball:reset()
			ball.dx = -100
			servingPlayer = 2

			if player1Score >= 3 then
				gameState = 'victory'
				winningPlayer = 1
			else
				gameState = 'serve'
			end
		end
	end
end


function love.keypressed(key)
	if key == 'escape' then
		love.event.quit()
	elseif key == 'enter' or key == 'return' then
		if gameState == 'start' then
			gameState = 'serve' 
		elseif gameState == 'serve' then
			gameState = 'play'
		elseif gameState == 'victory' then
			gameState = 'start'
			player1Score = 0
			player2Score = 0
		end
	end
end

function love.draw()

	push:apply('start')
	
	love.graphics.clear(40/255, 45/255, 52/255, 255/255)

	displayScore()

	if gameState == 'start' then 
		love.graphics.setFont(smallFont) 
		love.graphics.printf("Hello Pong!", 0, 10, VIRTUAL_WIDTH, 'center')
		love.graphics.printf("Press Enter to play.", 0, 20, VIRTUAL_WIDTH, 'center')
	elseif gameState == 'serve' then
		love.graphics.setFont(smallFont) 
		love.graphics.printf('Player ' .. tostring(servingPlayer) .. "'s turn!", 0, 10, VIRTUAL_WIDTH, 'center')
		love.graphics.printf("Press Enter to serve.", 0, 20, VIRTUAL_WIDTH, 'center')
	elseif gameState == 'victory' then
		love.graphics.setFont(victoryFont)
		love.graphics.printf('Player ' .. tostring(winningPlayer) .. " wins!", 0, 10, VIRTUAL_WIDTH, 'center')
		love.graphics.setFont(smallFont) 
		love.graphics.printf("Press Enter to restart.", 0, 42, VIRTUAL_WIDTH, 'center')
	end

	paddle1:render()
	paddle2:render()

	ball:render()

	displayFPS()

	push:apply('end')
end

function displayFPS()
    love.graphics.setFont(smallFont)
    love.graphics.setColor(0, 255/255, 0, 255/255)
    love.graphics.print('FPS: ' .. tostring(love.timer.getFPS()), 40, 20)
    love.graphics.setColor(1, 1, 1, 1)
end


function displayScore()
    love.graphics.setFont(scoreFont)
    love.graphics.print(tostring(player1Score), VIRTUAL_WIDTH / 2 - 50, 
        VIRTUAL_HEIGHT / 3)
    love.graphics.print(tostring(player2Score), VIRTUAL_WIDTH / 2 + 30,
        VIRTUAL_HEIGHT / 3)
end