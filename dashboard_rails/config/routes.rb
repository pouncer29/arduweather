Rails.application.routes.draw do
  get 'weather_data/index'
  get 'weather_data/current'
  get 'weather_data/week'
  get 'weather_data/month'
  get 'weather_data/year'
  # Define your application routes per the DSL in https://guides.rubyonrails.org/routing.html

  # Defines the root path route ("/")
  # root "articles#index"
end
