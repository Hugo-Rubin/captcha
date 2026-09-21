function [ f, d ] = Sift( img )
%SIFT Summary of this function goes here
%   Detailed explanation goes here

if ndims(img) == 3
    gs = Grayscale(img);
else
    gs = img;
end

I = single(gs);

[f, d] = vl_sift(I);

imshow(gs), title('Sample of Features in the Image (not all features are shown)');
hold on;
perm = randperm(size(f,2)) ; 
sel = perm(1:3) ;
h1 = vl_plotframe(f(:,sel)) ; 
h2 = vl_plotframe(f(:,sel)) ; 
set(h1, 'color', 'm', 'linewidth', 3) ;
set(h2, 'color', 'g', 'linewidth', 2) ;

h3 = vl_plotsiftdescriptor(d(:,sel),f(:,sel)) ;  
set(h3,'color','c');

end

