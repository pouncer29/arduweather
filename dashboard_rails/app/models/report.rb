class Report
  include Mongoid::Document
  include Mongoid::Timestamps
  field :temp, type: Float
  field :humidity, type: Float
  field :windSpeed, type: Float
  field :brightness, type: Float
  field :windDirection, type: String
end
