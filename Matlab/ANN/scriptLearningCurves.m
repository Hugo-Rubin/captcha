%% Initialization
clear ; close all; clc

%% Setup the parameters you will use for this exercise
%input_layer_size  = 3600;  % 60x60 Input Images of Digits
%hidden_layer_size = 261;   % x hidden units
%num_labels = 58;          % 58 labels, from 1 to 10, from a to Z -I,O,0,l


%% =========== Part 1: Loading and Visualizing Data =============
%  We start the exercise by first loading and visualizing the dataset. 
%  You will be working with a dataset that contains handwritten digits.
%

% Load Training Data
fprintf('Loading and Visualizing Data ...\n')

%load('XFinal.mat');
%load('y');
load('XF3.mat');
load('y3.mat');

%CV e Test sets
load('Xval.mat');
load('yVal.mat');
load('Xtest.mat');
load('ytest.mat');

% y vetorizado
%load('yArray.mat');

% 115 hidden units
%load('ThetasF3.mat');

% 174 hidden units
%load('Thetas174huV2.mat');

% 203 hidden units
%load('Thetas203huV2.mat');

% 261 hidden units
load('Thetas261huV3.mat');

%y = yArray;
y = y + 1;
yVal = yVal + 1;
ytest = ytest + 1;

%load('trainedWeights.mat');

%m = size(X, 1);
m = size(Xval, 1);
input_layer_size  = size(Theta1, 2) - 1;  % 60x60 Input Images of Digits
hidden_layer_size = size(Theta1, 1);
num_labels = size(Theta2, 1);

%Randomly select 100 data points to display
%sel = randperm(size(X, 1));
%sel = sel(1:100);

%displayData(X(sel, :));

%fprintf('Program paused. Press enter to continue.\n');
%pause;                          
                        

nn_params = [Theta1(:) ; Theta2(:)];

%% ================ Part 6: Initializing Pameters ================
%  In this part of the exercise, you will be starting to implment a two
%  layer neural network that classifies digits. You will start by
%  implementing a function to initialize the weights of the neural network
%  (randInitializeWeights.m)

fprintf('\nInitializing Neural Network Parameters ...\n')

% Inicializar Thetas 1 e 2 com valores previamente calculados, lidos de um arquivo
initial_Theta1 = Theta1;
initial_Theta2 = Theta2;

% Inicializar Thetas 1 e 2 aleatoriamente
%initial_Theta1 = randInitializeWeights(input_layer_size, hidden_layer_size);
%initial_Theta2 = randInitializeWeights(hidden_layer_size, num_labels);

% Unroll parameters
initial_nn_params = [initial_Theta1(:) ; initial_Theta2(:)];


%% =========== Part X: Learning Curve for Linear Regression =============
%  Next, you should implement the learningCurve function. 
%
%  Write Up Note: Since the model is underfitting the data, we expect to
%                 see a graph with "high bias" -- slide 8 in ML-advice.pdf 
%

lambda = 0;
[error_train, error_val] = ...
    learningCurve(initial_nn_params, input_layer_size, hidden_layer_size, ...
    num_labels, X(1:m, :), y(1:m, :), Xval, yVal, ...
                  lambda);

plot(1:m, error_train, 1:m, error_val);
title('Learning curve for neural network')
legend('Train', 'Cross Validation')
xlabel('Number of training examples')
ylabel('Error')
axis([0 13 0 150])

fprintf('# Training Examples\tTrain Error\tCross Validation Error\n');
for i = 1:m
    fprintf('  \t%d\t\t%f\t%f\n', i, error_train(i), error_val(i));
end
