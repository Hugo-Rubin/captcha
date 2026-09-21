function [ grad_img ] = Gradient_Img( ve_img, he_img )
%GRADIENT_IMG Recebe a arestas verticais e horizontais e as exibe na mesma
%imagem

grad_img = ((ve_img(:, 2:end-1) .^ 2) + (he_img(2:end-1, :)) .^ 2) .^ 1/2;

figure, imshow(grad_img), title('Gradient Image (Horizontal + Vertical Edges');

end

