class WeatherDataController < ApplicationController
  def index
  end

  def current
    @last = Report.last
    @latest = @last.attributes
  end

  def week
  end

  def month
  end

  def year
  end
end
