function [ ve_img ] = Vertical_ED( img )
%Vertical_ED Aplica a detecção de arestas verticais na imagem recebida

if ndims(img) == 3
    ve_img = Grayscale(img);
else
    ve_img = img;
end

[r, c] = size(ve_img);

ve_img = [zeros(r, 1) ve_img zeros(r, 1)];

for x = 1:r
    for y = 2:c
        ve_img(x, y) = ve_img(x, y + 1) - ve_img(x, y);
    end
end
   
figure, imshow(ve_img), title('Vertical Edges');
        
end

