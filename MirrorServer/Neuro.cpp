//warning not reintrant
#include <iostream>
#include <vector>
using namespace std;

vector<vector<vector<double > > > weights;
vector<vector<double> > values;
vector<vector<double> > error;
vector<vector<vector<double> > > weighterror;
float learnrate;

void setup(vector<int> layerLayout, float learnspeed) {
	learnrate=learnspeed;
	weighterror.resize(layerLayout.size());
	weights.resize(layerLayout.size());


	values.resize(layerLayout.size());
	for (int i = 0; i < layerLayout.size(); i++) {
		values[i].resize(layerLayout[i]);
	}

	error.resize(layerLayout.size());
	for (int i = 0; i < layerLayout.size(); i++) {
		error[i].resize(layerLayout[i]);
	}

	for (int i = 0; i < layerLayout.size(); i++) {
		weights[i].resize(layerLayout[i]);
		weighterror[i].resize(layerLayout[i]);

		for (int j = 0; j < weights[i].size(); j++) {

			weights[i][j].resize(layerLayout[i - 1]);
			weighterror[i][j].resize(layerLayout[i - 1]);

			for (int k = 0; k<weights[i][j].size(); k++) {
				weights[i][j][k] = 1.0f / weights[i][j].size();
				weighterror[i][j][k]=0;
			}
		}
	}
}
void predict(std::vector<float> input) {
	for (int i = 0; i < input.size(); i++) {
		values[0][i] = input[i];
	}

	for (int i = 1; i < weights.size(); i++) {
		for (int j = 0; j < weights[i].size(); j++) {

			values[i][j] = 0;
			for (int k = 0; k < weights[i][j].size(); k++) {
				values[i][j] = values[i][j]
						+ values[i - 1][k] * weights[i ][j][k];

			}
		}
	}
}
void calculateError(std::vector<float> desired) {
	for (int j = 0; j < desired.size(); j++) {
		error[error.size() - 1][j] = values[error.size() - 1][j] - desired[j];

	}



	for (int i = error.size() - 2; i > -1; i--) {

		for (int j = 0; j < error[i].size(); j++) {


			for (int k = 0; k < error[i + 1].size(); k++) {
				weighterror[i + 1][j][k] = weights[i + 1][j][k]
						* error[i + 1][k];
				error[i][j] = error[i][j] + weighterror[i + 1][j][k];

			}

		}
	}

}

vector<double> getOutput() {
	return values[values.size() - 1];
}

void learn() {

	for (int i = 1; i < weighterror.size(); i++) {
		for (int j = 0; j < weighterror[i].size(); j++) {
			for (int k = 0;k< weighterror[i][j].size(); k++) {
				weights[i][j][k] = weights[i][j][k]
						- weighterror[i][j][k] * learnrate;


			}

		}

	}

}
int main() {
	std::cout<<"started"<<std::endl;
	int t[] = { 1, 1 };
	std::vector<int> v(t, t + 2);
	setup(v, 0.5);
	std::cout<<"predicting"<<std::endl;;
	float tt[] = { 1 };
	std::vector<float> vv(tt, tt + 1);
	predict(vv);
	std::cout<<"calculating error"<<std::endl;;

	float ttt[] = { 3 };
	std::vector<float> vvv(ttt, ttt + 1);
	calculateError(vvv);
	std::cout<<"learning"<<std::endl;;

	learn();
	std::cout<<"getting output: "<<std::endl;;
	predict(vv);

	vector<double> r;
	r= getOutput();
	for (int i=0;i<r.size();i++) {
		std::cout << r[i] << ' ';
	}
	return 1;
}

