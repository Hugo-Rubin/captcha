function [ px, py ] = autofit( I, orderx, trimx, ordery, trimy )
%AUTOFIT Produces fitted polynomial using x and y midpoints of the given image
%   Note: For isolated curve images, it is probably better to do this
%   manually with separate images so as to avoid unnecessary fitting
%   perturbation by having unnecessary lines in the curve image

px = curvefit(I, orderx, trimx);
outputx = imunwarp(output, px);
py = curvefit(imrotate(outputx, 90), ordery, trimy);

end

